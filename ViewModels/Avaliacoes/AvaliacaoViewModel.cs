using System.Collections.ObjectModel;
using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Conta;
using LOCATEM_DESKTOP.Models.Avaliacoes;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Models.Locacoes;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Locacoes;
using LOCATEM_DESKTOP.ViewModels.Base;
using MauiIcons.Material;

namespace LOCATEM_DESKTOP.ViewModels.Avaliacoes
{
    /// <summary>
    /// Controla a tela Minhas Avaliações do Portal do Locador.
    /// A lista nasce exclusivamente das locações finalizadas do locador autenticado, repetindo a regra do Web.
    /// </summary>
    public sealed class AvaliacaoViewModel : BaseViewModel
    {
        // Chaves das duas abas exibidas na página.
        private const string AbaPendentes = "pendentes";
        private const string AbaRealizadas = "realizadas";

        // Rotas já migradas que podem ser acessadas pelo AppHeader a partir desta página.
        private static readonly HashSet<string> RotasMigradas = new(StringComparer.OrdinalIgnoreCase)
        {
            "homeLocador",
            "minhasFerramentas",
            "gerenciarLocacoes",
            "historicoLocacoes",
            "avaliacao",
            "perfil",
            "cadastroFerramenta",
            "ferramentaDetalhe"
        };

        // Serviços reutilizados: sessão identifica o locador e locações são a fonte única dos dados avaliáveis.
        private readonly IAuthSessionService _authSession;
        private readonly ILocacaoService _locacaoService;

        // Mantém todos os itens avaliáveis antes de aplicar a aba selecionada.
        private IReadOnlyList<ProdutoAvaliacao> _todosProdutos = Array.Empty<ProdutoAvaliacao>();

        // Aba inicial igual ao Web: avaliações que ainda precisam ser enviadas.
        private string _abaAtiva = AbaPendentes;

        public AvaliacaoViewModel(
            IAuthSessionService authSession,
            ILocacaoService locacaoService)
        {
            _authSession = authSession;
            _locacaoService = locacaoService;

            // Navegação compartilhada pelo AppHeader.
            NavegarCommand = new AsyncRelayCommand(async parametro =>
                await NavegarAsync(parametro as string));

            // Alterna entre as abas Pendentes e Realizadas.
            SelecionarAbaCommand = new RelayCommand(parametro =>
                SelecionarAba(parametro as string));

            // Abre o modal pelo clique no card.
            AbrirModalCommand = new RelayCommand(parametro =>
            {
                if (parametro is ProdutoAvaliacao produto)
                    AbrirModal(produto);
            });

            // Clique direto na estrela do card salva apenas o rascunho visual e abre o modal.
            SelecionarNotaGlobalCommand = new RelayCommand(parametro =>
            {
                if (parametro is SelecaoEstrela { Contexto: ProdutoAvaliacao produto } selecao)
                {
                    produto.NotaGlobal = selecao.Nota;
                    AbrirModal(produto);
                }
            });

            // Atualiza uma das três notas obrigatórias da perspectiva do locador.
            SelecionarAspectoNotaCommand = new RelayCommand(parametro =>
            {
                if (parametro is SelecaoEstrela { Contexto: AvaliacaoAspectoItem aspecto } selecao)
                {
                    aspecto.Nota = selecao.Nota;
                    aspecto.TemErro = false;
                    AtualizarEstadoErro();
                }
            });

            // Fecha o modal sem persistir o rascunho atual.
            FecharModalCommand = new RelayCommand(FecharModal);

            // Envia ou edita a avaliação depois de validar todas as subnotas obrigatórias.
            EnviarAvaliacaoCommand = new AsyncRelayCommand(EnviarAvaliacaoAsync);

            // O carrossel reutiliza a mesma abertura do modal para trocar rapidamente de locação pendente.
            SelecionarItemCarrosselCommand = new RelayCommand(parametro =>
            {
                if (parametro is ProdutoAvaliacao produto)
                    AbrirModal(produto);
            });
        }

        // Commands públicos consumidos pela página, cards, estrelas e modal.
        public ICommand NavegarCommand { get; }
        public ICommand SelecionarAbaCommand { get; }
        public ICommand AbrirModalCommand { get; }
        public ICommand SelecionarNotaGlobalCommand { get; }
        public ICommand SelecionarAspectoNotaCommand { get; }
        public ICommand FecharModalCommand { get; }
        public ICommand EnviarAvaliacaoCommand { get; }
        public ICommand SelecionarItemCarrosselCommand { get; }

        // Dados do usuário exibidos pelo AppHeader.
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

        // Coleção filtrada pela aba atual e renderizada no corpo da página.
        private IReadOnlyList<ProdutoAvaliacao> _produtos = Array.Empty<ProdutoAvaliacao>();
        public IReadOnlyList<ProdutoAvaliacao> Produtos
        {
            get => _produtos;
            private set => SetProperty(ref _produtos, value);
        }

        // Propriedades visuais das abas evitam lógica de comparação dentro do XAML.
        public bool PendentesAtiva => _abaAtiva == AbaPendentes;
        public bool RealizadasAtiva => _abaAtiva == AbaRealizadas;

        // Estado vazio da aba atual.
        private bool _temProdutos;
        public bool TemProdutos
        {
            get => _temProdutos;
            private set
            {
                if (SetProperty(ref _temProdutos, value))
                    OnPropertyChanged(nameof(SemProdutos));
            }
        }

        public bool SemProdutos => !TemProdutos;

        // Textos do estado vazio mudam conforme a aba, seguindo as mensagens do Web.
        private string _estadoVazioTitulo = string.Empty;
        public string EstadoVazioTitulo
        {
            get => _estadoVazioTitulo;
            private set => SetProperty(ref _estadoVazioTitulo, value);
        }

        private string _estadoVazioDescricao = string.Empty;
        public string EstadoVazioDescricao
        {
            get => _estadoVazioDescricao;
            private set => SetProperty(ref _estadoVazioDescricao, value);
        }

        // Produto atualmente aberto no modal.
        private ProdutoAvaliacao? _produtoAtual;
        public ProdutoAvaliacao? ProdutoAtual
        {
            get => _produtoAtual;
            private set
            {
                if (SetProperty(ref _produtoAtual, value))
                    OnPropertyChanged(nameof(TextoBotaoEnviar));
            }
        }

        // O modal permanece totalmente controlado pelo ViewModel.
        private bool _isModalAberto;
        public bool IsModalAberto
        {
            get => _isModalAberto;
            private set => SetProperty(ref _isModalAberto, value);
        }

        // As três subavaliações são observáveis para refletir estrela e erro em tempo real.
        public ObservableCollection<AvaliacaoAspectoItem> Aspectos { get; } = new();

        // Outros itens pendentes aparecem no final do modal, como no carrossel Web.
        private IReadOnlyList<ProdutoAvaliacao> _itensCarrossel = Array.Empty<ProdutoAvaliacao>();
        public IReadOnlyList<ProdutoAvaliacao> ItensCarrossel
        {
            get => _itensCarrossel;
            private set
            {
                if (SetProperty(ref _itensCarrossel, value))
                    OnPropertyChanged(nameof(TemItensCarrossel));
            }
        }

        public bool TemItensCarrossel => ItensCarrossel.Count > 0;

        // Observação opcional vinculada ao Editor do modal.
        private string _observacaoRascunho = string.Empty;
        public string ObservacaoRascunho
        {
            get => _observacaoRascunho;
            set => SetProperty(ref _observacaoRascunho, value ?? string.Empty);
        }

        // Mensagem de validação aparece somente quando algum aspecto obrigatório está zerado.
        private bool _erroVisivel;
        public bool ErroVisivel
        {
            get => _erroVisivel;
            private set => SetProperty(ref _erroVisivel, value);
        }

        public string MensagemErro => "Avalie Locatário, Entrega e Plataforma antes de enviar.";

        // O botão muda para Editar quando o registro já existe, reproduzindo o modal Web.
        public string TextoBotaoEnviar => ProdutoAtual?.Status == StatusAvaliacao.Realizada ? "Editar" : "Enviar";

        // Toast de confirmação exibido após salvar ou editar com sucesso.
        private bool _toastVisivel;
        public bool ToastVisivel
        {
            get => _toastVisivel;
            private set => SetProperty(ref _toastVisivel, value);
        }

        /// <summary>Garante que somente o locador autenticado acesse a versão atual da tela Desktop.</summary>
        public async Task<bool> GarantirAcessoAsync()
        {
            var usuario = _authSession.UsuarioAtual;
            if (usuario is not null && usuario.Tipo == TipoUsuario.Locador)
                return true;

            await Shell.Current.GoToAsync("//login");
            return false;
        }

        /// <summary>Recarrega as avaliações a partir das locações finalizadas do locador.</summary>
        public void Carregar()
        {
            var usuario = _authSession.UsuarioAtual;
            if (usuario is null)
                return;

            // Atualiza as informações compartilhadas com o AppHeader.
            NomeUsuario = usuario.Nome;
            FotoUsuario = AvatarHelper.CriarImageSource(usuario.FotoUrl);

            // Somente locações finalizadas liberam avaliação; o filtro por LocadorId impede dados de outras lojas.
            _todosProdutos = _locacaoService
                .ObterPorLocador(usuario.LocadorId)
                .Where(locacao => locacao.Status == StatusLocacao.Finalizada)
                .Select(CriarProdutoAvaliacao)
                .ToList();

            AtualizarLista();
        }

        // Converte a locação existente no formato de apresentação da página sem duplicar a fonte de dados.
        private static ProdutoAvaliacao CriarProdutoAvaliacao(Locacao locacao)
        {
            var registro = locacao.AvaliacaoDoLocador;

            return new ProdutoAvaliacao
            {
                LocacaoId = locacao.Id,
                Nome = locacao.Produto,
                DataLocacao = $"Locado em {locacao.Periodo}",
                Imagem = locacao.Imagem,
                LojaNome = locacao.Locador,
                LojaLogo = ObterLogoLoja(locacao.LocadorId),
                Locatario = locacao.Locatario,
                Status = registro is null ? StatusAvaliacao.Pendente : StatusAvaliacao.Realizada,
                NotaGlobal = registro?.NotaGlobal ?? 0
            };
        }

        // Mapeamento dos assets de logo já existentes em Resources/Images/LogosLojas.
        private static string? ObterLogoLoja(string locadorId) => locadorId switch
        {
            "loc-jb" => "logo_loja_jb.png",
            "loc-ms" => "logo_loja_ms.png",
            _ => null
        };

        // Troca a aba apenas quando a chave recebida é válida e diferente da atual.
        private void SelecionarAba(string? aba)
        {
            if (aba is not (AbaPendentes or AbaRealizadas) || aba == _abaAtiva)
                return;

            _abaAtiva = aba;
            OnPropertyChanged(nameof(PendentesAtiva));
            OnPropertyChanged(nameof(RealizadasAtiva));
            AtualizarLista();
        }

        // Aplica o filtro da aba e prepara os textos do estado vazio correspondente.
        private void AtualizarLista()
        {
            var status = PendentesAtiva ? StatusAvaliacao.Pendente : StatusAvaliacao.Realizada;
            Produtos = _todosProdutos.Where(produto => produto.Status == status).ToList();
            TemProdutos = Produtos.Count > 0;

            if (PendentesAtiva)
            {
                EstadoVazioTitulo = "Parabéns, você está em dia!";
                EstadoVazioDescricao = "Nenhuma avaliação pendente por aqui.";
            }
            else
            {
                EstadoVazioTitulo = "Ainda sem avaliações realizadas.";
                EstadoVazioDescricao = "Suas avaliações enviadas aparecerão aqui.";
            }
        }

        // Abre o modal e carrega notas já salvas ou cria o rascunho zerado para uma nova avaliação.
        private void AbrirModal(ProdutoAvaliacao produto)
        {
            ProdutoAtual = produto;
            Aspectos.Clear();

            var locacao = _locacaoService.Locacoes.FirstOrDefault(item => item.Id == produto.LocacaoId);
            var registro = locacao?.AvaliacaoDoLocador;

            Aspectos.Add(CriarAspecto(AspectoAvaliacao.Locatario, "Avaliação Locatário", MaterialIcons.Person, registro));
            Aspectos.Add(CriarAspecto(AspectoAvaliacao.Entrega, "Avaliação Entrega", MaterialIcons.LocalShipping, registro));
            Aspectos.Add(CriarAspecto(AspectoAvaliacao.Plataforma, "Avaliação Plataforma", MaterialIcons.Devices, registro));

            ObservacaoRascunho = registro?.Observacao ?? string.Empty;
            ErroVisivel = false;
            ItensCarrossel = _todosProdutos
                .Where(item => item.Status == StatusAvaliacao.Pendente && item.LocacaoId != produto.LocacaoId)
                .ToList();
            IsModalAberto = true;
        }

        // Monta uma linha do modal recuperando a nota previamente salva quando houver edição.
        private static AvaliacaoAspectoItem CriarAspecto(
            AspectoAvaliacao aspecto,
            string label,
            MaterialIcons icone,
            RegistroAvaliacao? registro)
        {
            var nota = registro is not null && registro.SubAvaliacoes.TryGetValue(aspecto, out var valor)
                ? valor
                : 0;

            return new AvaliacaoAspectoItem
            {
                Aspecto = aspecto,
                Label = label,
                Icone = icone,
                Nota = nota
            };
        }

        // Limpa o estado transitório para impedir que um rascunho apareça em outra locação.
        private void FecharModal()
        {
            IsModalAberto = false;
            ProdutoAtual = null;
            Aspectos.Clear();
            ItensCarrossel = Array.Empty<ProdutoAvaliacao>();
            ObservacaoRascunho = string.Empty;
            ErroVisivel = false;
        }

        // Recalcula a mensagem de erro após cada clique em estrela.
        private void AtualizarEstadoErro() =>
            ErroVisivel = Aspectos.Any(aspecto => aspecto.TemErro);

        // Valida e persiste a avaliação na própria locação, exatamente como o fluxo Web faz no LocacaoStore.
        private async Task EnviarAvaliacaoAsync()
        {
            if (ProdutoAtual is null)
                return;

            var possuiErro = false;
            foreach (var aspecto in Aspectos)
            {
                aspecto.TemErro = aspecto.Nota <= 0;
                possuiErro |= aspecto.TemErro;
            }

            ErroVisivel = possuiErro;
            if (possuiErro)
                return;

            var notas = Aspectos.Select(aspecto => aspecto.Nota).ToList();
            var media = (int)Math.Round(notas.Average(), MidpointRounding.AwayFromZero);

            var registro = new RegistroAvaliacao
            {
                NotaGlobal = media,
                Observacao = ObservacaoRascunho.Trim(),
                SubAvaliacoes = Aspectos.ToDictionary(aspecto => aspecto.Aspecto, aspecto => aspecto.Nota)
            };

            _locacaoService.AtualizarLocacao(
                ProdutoAtual.LocacaoId,
                locacao => locacao.AvaliacaoDoLocador = registro);

            FecharModal();
            Carregar();

            // Confirma visualmente o envio e remove a mensagem depois do mesmo intervalo usado no Web.
            ToastVisivel = true;
            await Task.Delay(3000);
            ToastVisivel = false;
        }

        // Navegação do cabeçalho protege contra rotas não migradas e evita abrir a página atual novamente.
        private static async Task NavegarAsync(string? rota)
        {
            if (string.IsNullOrWhiteSpace(rota) || !RotasMigradas.Contains(rota))
                return;

            if (Shell.Current.CurrentState.Location.OriginalString.EndsWith(rota, StringComparison.OrdinalIgnoreCase))
                return;

            if (string.Equals(rota, "homeLocador", StringComparison.OrdinalIgnoreCase))
                await Shell.Current.GoToAsync("//homeLocador");
            else
                await Shell.Current.GoToAsync(rota);
        }
    }
}
