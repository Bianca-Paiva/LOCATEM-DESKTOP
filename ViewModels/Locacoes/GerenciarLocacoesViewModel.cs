using System.Windows.Input;
using LOCATEM_DESKTOP.Components.Shared;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Conta;
using LOCATEM_DESKTOP.Helpers.Locacoes;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Models.Locacoes;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Locacoes;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Locacoes
{
    /// <summary>
    /// Estado e regras da tela Gerenciar Locações (visão do locador), migrados de
    /// GerenciarLocacoes.tsx + useGerenciarLocacoes.ts.
    /// </summary>
    public class GerenciarLocacoesViewModel : BaseViewModel
    {
        private const string FiltroEmAberto = "emAberto";
        private const int PrazoPagamentoHoras = 24;

        private static readonly (string Chave, string Label)[] DefinicaoAbas =
        {
            (FiltroEmAberto, "Em aberto"),
            ("pendente", "Pendente"),
            ("aguardandoPagamento", "Aguardando pagamento"),
            ("preparandoEntrega", "Preparando entrega"),
            ("emTransporte", "Em transporte"),
            ("emAndamento", "Em andamento"),
            ("aguardandoDevolucao", "Aguardando devolução"),
            ("devolucaoEmTransporte", "Devolução em transporte")
        };

        private static readonly HashSet<string> RotasMigradas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "homeLocador",
                "minhasFerramentas",
                "gerenciarLocacoes",
                "perfil",
                "cadastroFerramenta",
                "ferramentaDetalhe"
            };

        private readonly IAuthSessionService _authSession;
        private readonly ILocacaoService _locacaoService;
        private IReadOnlyList<Locacao> _locacoesEmAberto = Array.Empty<Locacao>();
        private string _filtro = FiltroEmAberto;
        private Locacao? _locacaoParaRecusar;

        public GerenciarLocacoesViewModel(
            IAuthSessionService authSession,
            ILocacaoService locacaoService)
        {
            _authSession = authSession;
            _locacaoService = locacaoService;

            NavegarCommand = new AsyncRelayCommand(async parametro =>
                await NavegarAsync(parametro as string));

            SelecionarAbaCommand = new RelayCommand(parametro =>
                SelecionarAba(parametro as string));

            AbrirSolicitacaoCommand = new RelayCommand(parametro =>
                AbrirSolicitacao(parametro as Locacao));

            FecharModalCommand = new RelayCommand(FecharModal);

            AprovarCommand = new RelayCommand(parametro =>
                Aprovar(parametro as Locacao));

            SolicitarRecusaCommand = new RelayCommand(parametro =>
                AbrirConfirmacaoRecusa(parametro as Locacao));

            ConfirmarRecusaCommand = new RelayCommand(ConfirmarRecusa);
            CancelarRecusaCommand = new RelayCommand(CancelarRecusa);
        }

        public ICommand NavegarCommand { get; }
        public ICommand SelecionarAbaCommand { get; }
        public ICommand AbrirSolicitacaoCommand { get; }
        public ICommand FecharModalCommand { get; }
        public ICommand AprovarCommand { get; }
        public ICommand SolicitarRecusaCommand { get; }
        public ICommand ConfirmarRecusaCommand { get; }
        public ICommand CancelarRecusaCommand { get; }

        private string _nomeUsuario = string.Empty;
        public string NomeUsuario
        {
            get => _nomeUsuario;
            private set => SetProperty(ref _nomeUsuario, value);
        }

        private ImageSource? _fotoUsuario;
        public ImageSource? FotoUsuario
        {
            get => _fotoUsuario;
            private set => SetProperty(ref _fotoUsuario, value);
        }

        private IReadOnlyList<AbaItem> _abas = Array.Empty<AbaItem>();
        public IReadOnlyList<AbaItem> Abas
        {
            get => _abas;
            private set => SetProperty(ref _abas, value);
        }

        private IReadOnlyList<Locacao> _locacoes = Array.Empty<Locacao>();
        public IReadOnlyList<Locacao> Locacoes
        {
            get => _locacoes;
            private set => SetProperty(ref _locacoes, value);
        }

        private bool _temLocacoes;
        public bool TemLocacoes
        {
            get => _temLocacoes;
            private set
            {
                if (SetProperty(ref _temLocacoes, value))
                    OnPropertyChanged(nameof(SemLocacoes));
            }
        }

        public bool SemLocacoes => !TemLocacoes;

        private Locacao? _locacaoModal;
        public Locacao? LocacaoModal
        {
            get => _locacaoModal;
            private set
            {
                if (SetProperty(ref _locacaoModal, value))
                    OnPropertyChanged(nameof(IsModalAprovacaoAberto));
            }
        }

        public bool IsModalAprovacaoAberto => LocacaoModal is not null;

        private bool _isConfirmandoRecusa;
        public bool IsConfirmandoRecusa
        {
            get => _isConfirmandoRecusa;
            private set => SetProperty(ref _isConfirmandoRecusa, value);
        }

        private string _mensagemConfirmacaoRecusa = string.Empty;
        public string MensagemConfirmacaoRecusa
        {
            get => _mensagemConfirmacaoRecusa;
            private set => SetProperty(ref _mensagemConfirmacaoRecusa, value);
        }

        public async Task<bool> GarantirAcessoAsync()
        {
            var usuario = _authSession.UsuarioAtual;
            if (usuario is not null && usuario.Tipo == TipoUsuario.Locador)
                return true;

            await Shell.Current.GoToAsync("//login");
            return false;
        }

        public void Carregar()
        {
            var usuario = _authSession.UsuarioAtual;
            if (usuario is null)
                return;

            NomeUsuario = usuario.Nome;
            FotoUsuario = AvatarHelper.CriarImageSource(usuario.FotoUrl);

            _locacaoService.CancelarPagamentosVencidos();

            _locacoesEmAberto = _locacaoService
                .ObterPorLocador(usuario.LocadorId)
                .Where(l => !LocacaoGerenciamentoHelper.EhEncerrada(l.Status))
                .ToList();

            AtualizarTela();
        }

        public void VerificarPrazosPagamento()
        {
            if (_locacaoService.CancelarPagamentosVencidos())
                Carregar();
        }

        private void SelecionarAba(string? chave)
        {
            if (string.IsNullOrWhiteSpace(chave) ||
                !DefinicaoAbas.Any(a => a.Chave == chave) ||
                _filtro == chave)
            {
                return;
            }

            _filtro = chave;
            AtualizarTela();
        }

        private void AtualizarTela()
        {
            var contagem = DefinicaoAbas.ToDictionary(a => a.Chave, _ => 0);
            contagem[FiltroEmAberto] = _locacoesEmAberto.Count;

            foreach (var locacao in _locacoesEmAberto)
            {
                var chave = LocacaoGerenciamentoHelper.ObterChaveFiltro(locacao.Status);
                if (chave is not null && contagem.ContainsKey(chave))
                    contagem[chave]++;
            }

            Abas = DefinicaoAbas
                .Select(a => new AbaItem(
                    a.Chave,
                    a.Label,
                    contagem[a.Chave],
                    a.Chave == _filtro))
                .ToList();

            Locacoes = _filtro == FiltroEmAberto
                ? _locacoesEmAberto.ToList()
                : _locacoesEmAberto
                    .Where(l => LocacaoGerenciamentoHelper.ObterChaveFiltro(l.Status) == _filtro)
                    .ToList();

            TemLocacoes = Locacoes.Count > 0;
        }

        private void AbrirSolicitacao(Locacao? locacao)
        {
            if (locacao is null || locacao.Status != StatusLocacao.Pendente)
                return;

            LocacaoModal = locacao;
        }

        private void FecharModal()
        {
            if (IsConfirmandoRecusa)
                return;

            LocacaoModal = null;
        }

        private void Aprovar(Locacao? locacao)
        {
            if (locacao is null || locacao.Status != StatusLocacao.Pendente)
                return;

            _locacaoService.AtualizarLocacao(locacao.Id, atual =>
            {
                atual.Status = StatusLocacao.AguardandoPagamento;
                atual.MensagemStatus = "Locação aceita, aguardando o pagamento do locatário";
                atual.PrazoPagamento = DateTimeOffset.Now.AddHours(PrazoPagamentoHoras);
            });

            LocacaoModal = null;
            Carregar();
        }

        private void AbrirConfirmacaoRecusa(Locacao? locacao)
        {
            if (locacao is null || locacao.Status != StatusLocacao.Pendente)
                return;

            _locacaoParaRecusar = locacao;
            MensagemConfirmacaoRecusa =
                $"Tem certeza que deseja recusar a solicitação de \"{locacao.Produto}\"? Esta ação não pode ser desfeita.";
            IsConfirmandoRecusa = true;
        }

        private void CancelarRecusa()
        {
            _locacaoParaRecusar = null;
            IsConfirmandoRecusa = false;
        }

        private void ConfirmarRecusa()
        {
            if (_locacaoParaRecusar is null)
            {
                CancelarRecusa();
                return;
            }

            var id = _locacaoParaRecusar.Id;
            _locacaoService.AtualizarLocacao(id, atual =>
            {
                atual.Status = StatusLocacao.Recusada;
                atual.MensagemStatus = "Solicitação recusada pelo locador";
                atual.MotivoRecusa = "Solicitação recusada pelo locador";
                atual.PrazoPagamento = null;
            });

            _locacaoParaRecusar = null;
            IsConfirmandoRecusa = false;
            LocacaoModal = null;
            Carregar();
        }

        private static async Task NavegarAsync(string? rota)
        {
            if (string.IsNullOrWhiteSpace(rota) || !RotasMigradas.Contains(rota))
                return;

            if (Shell.Current.CurrentState.Location.OriginalString.EndsWith(
                    rota,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.Equals(rota, "homeLocador", StringComparison.OrdinalIgnoreCase))
                await Shell.Current.GoToAsync("//homeLocador");
            else
                await Shell.Current.GoToAsync(rota);
        }
    }
}
