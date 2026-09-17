using System.Collections.ObjectModel;
using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Formatacao;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Models.Ferramentas;
using LOCATEM_DESKTOP.Models.Home;
using LOCATEM_DESKTOP.Models.Locacoes;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Ferramentas;
using LOCATEM_DESKTOP.Services.Locacoes;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Home
{
    //
    // Migrado de pages/Home/HomeLocador/HomeLocador.tsx + hooks/Home/useHomeLocador.ts.
    // Reúne, calcula e formata os dados da Home do Locador sempre a partir das fontes já existentes
    // (catálogo, locações e usuário autenticado), nunca de valores fixos próprios desta tela.
    //
    public class HomeLocadorViewModel : BaseViewModel
    {
        // Quantos itens cada seção mostra antes do "Ver todas".
        private const int LimiteMinhasFerramentas = 4;
        private const int LimiteSolicitacoesRecentes = 4;
        private const int LimiteAgendaSemana = 4;

        // Locação paga, mas a ferramenta ainda está indo do locador para o locatário.
        private static readonly StatusLocacao[] StatusColetaParaEntrega =
            { StatusLocacao.Confirmada, StatusLocacao.PreparandoEntrega, StatusLocacao.EmTransporte };

        // Ferramenta já usada pelo locatário e a caminho de volta ao locador.
        private static readonly StatusLocacao[] StatusRetornoAoLocador =
            { StatusLocacao.AguardandoDevolucao, StatusLocacao.DevolucaoEmTransporte };

        // Rotas já migradas para o MAUI. As demais telas do locador (Minhas Ferramentas, Gerenciar
        // Locações, Detalhe/Cadastro de Ferramenta) ainda não existem, então a navegação fica inativa
        // até que sejam migradas — mesma NOTA DE ESCOPO já usada no LoginViewModel.
        private static readonly HashSet<string> RotasMigradas =
            new(StringComparer.OrdinalIgnoreCase) { "login", "cadastro", "informeEmail", "informeToken", "informeNovaSenha", "homeLocador" };

        private readonly IAuthSessionService _authSession;
        private readonly ICatalogoService _catalogo;
        private readonly ILocacaoService _locacoes;

        public HomeLocadorViewModel(IAuthSessionService authSession, ICatalogoService catalogo, ILocacaoService locacoes)
        {
            _authSession = authSession;
            _catalogo = catalogo;
            _locacoes = locacoes;

            NavegarCommand = new AsyncRelayCommand(async parametro => await NavegarAsync(parametro as string));
            VerTodasSolicitacoesCommand = new AsyncRelayCommand(async () => await NavegarAsync("gerenciarLocacoes"));
            VerDetalhesSolicitacaoCommand = new AsyncRelayCommand(async () => await NavegarAsync("gerenciarLocacoes"));
            VerTodasFerramentasCommand = new AsyncRelayCommand(async () => await NavegarAsync("minhasFerramentas"));
            VerFerramentaCommand = new AsyncRelayCommand(async parametro => await AbrirFerramentaAsync(parametro, "ferramentaDetalhe"));
            EditarFerramentaCommand = new AsyncRelayCommand(async parametro => await AbrirFerramentaAsync(parametro, "cadastroFerramenta"));
            CadastrarFerramentaCommand = new AsyncRelayCommand(async () =>
            {
                _catalogo.FerramentaSelecionadaId = null;
                await NavegarAsync("cadastroFerramenta");
            });
        }

        public ObservableCollection<Locacao> SolicitacoesRecentes { get; } = new();
        public ObservableCollection<AgendaSemanaLocadorItem> AgendaSemana { get; } = new();
        public ObservableCollection<ProdutoHome> MinhasFerramentas { get; } = new();

        public ICommand NavegarCommand { get; }
        public ICommand VerTodasSolicitacoesCommand { get; }
        public ICommand VerDetalhesSolicitacaoCommand { get; }
        public ICommand VerTodasFerramentasCommand { get; }
        public ICommand VerFerramentaCommand { get; }
        public ICommand EditarFerramentaCommand { get; }
        public ICommand CadastrarFerramentaCommand { get; }

        private string _nomeUsuario = string.Empty;
        public string NomeUsuario { get => _nomeUsuario; private set => SetProperty(ref _nomeUsuario, value); }

        private string _saudacao = string.Empty;
        public string Saudacao { get => _saudacao; private set => SetProperty(ref _saudacao, value); }

        public string Subtitulo => "Aqui está um resumo das suas ferramentas, locações e desempenho da conta.";

        private string _ferramentasAtivas = "0";
        public string FerramentasAtivas { get => _ferramentasAtivas; private set => SetProperty(ref _ferramentasAtivas, value); }

        private string _ferramentasTendencia = string.Empty;
        public string FerramentasTendencia { get => _ferramentasTendencia; private set => SetProperty(ref _ferramentasTendencia, value); }

        private string _locacoesEmAndamento = "0";
        public string LocacoesEmAndamento { get => _locacoesEmAndamento; private set => SetProperty(ref _locacoesEmAndamento, value); }

        private string _locacoesLegenda = string.Empty;
        public string LocacoesLegenda { get => _locacoesLegenda; private set => SetProperty(ref _locacoesLegenda, value); }

        private string _solicitacoesPendentes = "0";
        public string SolicitacoesPendentes { get => _solicitacoesPendentes; private set => SetProperty(ref _solicitacoesPendentes, value); }

        private string _faturamentoMes = string.Empty;
        public string FaturamentoMes { get => _faturamentoMes; private set => SetProperty(ref _faturamentoMes, value); }

        private string _faturamentoTendencia = string.Empty;
        public string FaturamentoTendencia { get => _faturamentoTendencia; private set => SetProperty(ref _faturamentoTendencia, value); }

        private string _avaliacaoMedia = "0,0";
        public string AvaliacaoMedia { get => _avaliacaoMedia; private set => SetProperty(ref _avaliacaoMedia, value); }

        private double _avaliacaoNota;
        public double AvaliacaoNota { get => _avaliacaoNota; private set => SetProperty(ref _avaliacaoNota, value); }

        private string _totalAvaliacoes = string.Empty;
        public string TotalAvaliacoes { get => _totalAvaliacoes; private set => SetProperty(ref _totalAvaliacoes, value); }

        private bool _temSolicitacoes;
        public bool TemSolicitacoes
        {
            get => _temSolicitacoes;
            private set { if (SetProperty(ref _temSolicitacoes, value)) OnPropertyChanged(nameof(SemSolicitacoes)); }
        }
        public bool SemSolicitacoes => !TemSolicitacoes;

        private bool _temAgenda;
        public bool TemAgenda
        {
            get => _temAgenda;
            private set { if (SetProperty(ref _temAgenda, value)) OnPropertyChanged(nameof(SemAgenda)); }
        }
        public bool SemAgenda => !TemAgenda;

        /// <summary>
        /// Equivalente a useExigirPerfil(navigate, 'locador', ...): bloqueia visitante e usuário de
        /// outro tipo. No React o fallback é a Home do locatário, que ainda não foi migrada — aqui o
        /// usuário volta para o Login.
        /// </summary>
        public async Task<bool> GarantirAcessoAsync()
        {
            var usuario = _authSession.UsuarioAtual;
            if (usuario is not null && usuario.Tipo == TipoUsuario.Locador) return true;

            await Shell.Current.GoToAsync("//login");
            return false;
        }

        /// <summary>Recalcula tudo a partir das fontes de dados (equivale aos useMemo do hook React).</summary>
        public void Carregar()
        {
            var usuario = _authSession.UsuarioAtual;
            if (usuario is null) return;

            // Mesmo filtro das demais telas do locador: sempre pelo identificador, nunca pelo nome.
            var minhasFerramentas = _catalogo.ObterPorLocador(usuario.LocadorId);
            var minhasLocacoes = _locacoes.ObterPorLocador(usuario.LocadorId);

            AtualizarCabecalho(usuario);
            AtualizarResumo(usuario, minhasFerramentas, minhasLocacoes);
            AtualizarSolicitacoes(minhasLocacoes);
            AtualizarAgenda(minhasLocacoes);
            AtualizarFerramentas(minhasFerramentas);
        }

        private void AtualizarCabecalho(Usuario usuario)
        {
            NomeUsuario = usuario.Nome;
            Saudacao = $"Olá, {usuario.Nome.Split(' ').FirstOrDefault()}!";
        }

        private void AtualizarResumo(Usuario usuario, IReadOnlyList<Produto> ferramentas, IReadOnlyList<Locacao> locacoes)
        {
            var resumo = CalcularResumo(ferramentas, locacoes);

            FerramentasAtivas = resumo.FerramentasAtivas.ToString();
            // Nunca inventamos "+0": sem cadastro no mês, o texto simplesmente não aparece.
            FerramentasTendencia = resumo.FerramentasCadastradasEsteMes > 0
                ? $"+{resumo.FerramentasCadastradasEsteMes} este mês"
                : string.Empty;

            LocacoesEmAndamento = resumo.LocacoesEmAndamento.ToString();
            LocacoesLegenda = $"de {resumo.FerramentasAtivas} ferramentas ativas";

            SolicitacoesPendentes = resumo.SolicitacoesPendentes.ToString();

            FaturamentoMes = ValorMonetario.Formatar(resumo.FaturamentoMesAtual);
            FaturamentoTendencia = CalcularTendenciaFaturamento(resumo);

            AvaliacaoNota = usuario.Reputacao.Rating;
            AvaliacaoMedia = usuario.Reputacao.Rating.ToString("0.0");
            TotalAvaliacoes = $"({usuario.Reputacao.TotalAvaliacoes} avaliações)";
        }

        private static ResumoHomeLocador CalcularResumo(IReadOnlyList<Produto> ferramentas, IReadOnlyList<Locacao> locacoes)
        {
            var agora = DateTime.Now;
            var mesAnterior = new DateTime(agora.Year, agora.Month, 1).AddMonths(-1);

            // Mesma definição de "receita" usada em FerramentaDetalhe (soma das locações finalizadas),
            // aqui restrita ao mês de referência.
            decimal FaturamentoDoMes(int mes, int ano) => locacoes
                .Where(l => l.Status == StatusLocacao.Finalizada && EstaNoMes(l.DataInicio, mes, ano))
                .Sum(l => ValorMonetario.ParaNumero(l.Valor.Replace("R$", string.Empty).Trim()));

            return new ResumoHomeLocador
            {
                FerramentasAtivas = ferramentas.Count(f => f.Status != StatusFerramenta.Indisponivel),
                FerramentasCadastradasEsteMes = ferramentas.Count(f => EstaNoMes(f.CadastradoEm, agora.Month, agora.Year)),
                LocacoesEmAndamento = locacoes.Count(l => l.Status == StatusLocacao.EmAndamento),
                SolicitacoesPendentes = locacoes.Count(l => l.Status == StatusLocacao.Pendente),
                FaturamentoMesAtual = FaturamentoDoMes(agora.Month, agora.Year),
                FaturamentoMesAnterior = FaturamentoDoMes(mesAnterior.Month, mesAnterior.Year)
            };
        }

        private static string CalcularTendenciaFaturamento(ResumoHomeLocador resumo)
        {
            if (resumo.FaturamentoMesAnterior <= 0) return string.Empty;

            var percentual = (int)Math.Round(
                (resumo.FaturamentoMesAtual - resumo.FaturamentoMesAnterior) / resumo.FaturamentoMesAnterior * 100);

            return $"{(percentual >= 0 ? "+" : string.Empty)}{percentual}% em relação ao mês anterior";
        }

        private static bool EstaNoMes(string? dataBr, int mes, int ano)
        {
            var data = FormatoDataBr.ParaDataBr(dataBr);
            return data is not null && data.Value.Month == mes && data.Value.Year == ano;
        }

        private void AtualizarSolicitacoes(IReadOnlyList<Locacao> locacoes)
        {
            SolicitacoesRecentes.Clear();

            foreach (var locacao in locacoes.OrderByDescending(l => ChaveRecencia(l.Id)).Take(LimiteSolicitacoesRecentes))
                SolicitacoesRecentes.Add(locacao);

            TemSolicitacoes = SolicitacoesRecentes.Count > 0;
        }

        /// <summary>
        /// Ordena da solicitação mais recente para a mais antiga. Os ids do mock são numéricos
        /// sequenciais e os criados em runtime usam "r-&lt;timestamp&gt;" — sempre maiores, então
        /// ficam à frente sem precisar de um campo novo de data de criação.
        /// </summary>
        private static long ChaveRecencia(string id)
        {
            if (long.TryParse(id, out var numerico)) return numerico;

            var digitos = new string(id.Where(char.IsDigit).ToArray());
            return long.TryParse(digitos, out var valor) ? valor : 0;
        }

        private void AtualizarAgenda(IReadOnlyList<Locacao> locacoes)
        {
            var eventos = new List<AgendaSemanaLocadorItem>();

            foreach (var locacao in locacoes)
            {
                if (StatusColetaParaEntrega.Contains(locacao.Status))
                {
                    eventos.Add(new AgendaSemanaLocadorItem
                    {
                        Id = $"{locacao.Id}-coleta",
                        LocacaoId = locacao.Id,
                        Data = locacao.DataInicio,
                        Hora = locacao.HoraInicio,
                        Ferramenta = locacao.Produto,
                        Imagem = locacao.Imagem,
                        Locatario = locacao.Locatario,
                        TipoMovimento = TipoMovimentoLogisticoLocador.ColetaParaEntrega
                    });
                }

                if (StatusRetornoAoLocador.Contains(locacao.Status))
                {
                    eventos.Add(new AgendaSemanaLocadorItem
                    {
                        Id = $"{locacao.Id}-retorno",
                        LocacaoId = locacao.Id,
                        Data = locacao.DataFim,
                        Hora = locacao.HoraFim,
                        Ferramenta = locacao.Produto,
                        Imagem = locacao.Imagem,
                        Locatario = locacao.Locatario,
                        TipoMovimento = TipoMovimentoLogisticoLocador.RetornoAoLocador
                    });
                }
            }

            AgendaSemana.Clear();

            foreach (var evento in eventos
                .OrderBy(e => FormatoDataBr.ParaDataBr(e.Data) ?? DateTime.MinValue)
                .Take(LimiteAgendaSemana))
            {
                AgendaSemana.Add(evento);
            }

            TemAgenda = AgendaSemana.Count > 0;
        }

        private void AtualizarFerramentas(IReadOnlyList<Produto> ferramentas)
        {
            MinhasFerramentas.Clear();

            foreach (var produto in ferramentas.Take(LimiteMinhasFerramentas))
                MinhasFerramentas.Add(ProdutoHome.DeProduto(produto));
        }

        private async Task AbrirFerramentaAsync(object? parametro, string rota)
        {
            if (parametro is int id) _catalogo.FerramentaSelecionadaId = id;
            await NavegarAsync(rota);
        }

        private static async Task NavegarAsync(string? rota)
        {
            if (string.IsNullOrWhiteSpace(rota) || !RotasMigradas.Contains(rota)) return;

            // Tocar no item já ativo não deve empilhar a mesma página de novo.
            if (Shell.Current.CurrentState.Location.OriginalString.EndsWith(rota, StringComparison.OrdinalIgnoreCase)) return;

            await Shell.Current.GoToAsync(rota);
        }
    }
}
