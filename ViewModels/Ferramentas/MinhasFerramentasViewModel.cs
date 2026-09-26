using System.Windows.Input;
using LOCATEM_DESKTOP.Components.Ferramentas;
using LOCATEM_DESKTOP.Components.Shared;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Conta;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Models.Ferramentas;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Ferramentas;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Ferramentas
{
    //
    // Migrado de pages/Ferramentas/MinhasFerramentas/MinhasFerramentas.tsx.
    //
    // Estado da tela (React → MAUI):
    //   useCatalogoStore().produtos + useAuth().usuario  →  ICatalogoService + IAuthSessionService
    //   useState<FiltroFerramenta>('todas')              →  _filtro (null = "Todas")
    //   useMemo(contagem / ferramentasFiltradas)         →  AtualizarTela()
    //   navigate('ferramentaDetalhe' | 'cadastroFerramenta') → NavegarAsync(...)
    //
    // Os dados vêm SEMPRE de ICatalogoService (hoje alimentado pelo ProdutosMock). Para ligar
    // a API basta trocar a implementação do serviço — esta tela não precisa ser reconstruída.
    //
    public class MinhasFerramentasViewModel : BaseViewModel
    {
        // Chave da aba "Todas" (as demais abas usam o nome do StatusFerramenta).
        private const string ChaveTodas = "todas";

        // Ordem das abas de status — a mesma da versão Web.
        private static readonly StatusFerramenta[] OrdemAbas =
        {
            StatusFerramenta.Disponivel,
            StatusFerramenta.Locada,
            StatusFerramenta.Indisponivel,
            StatusFerramenta.Manutencao
        };

        // Rotas disponíveis no Desktop, incluindo o detalhe aberto pelo botão Ver.
        private static readonly HashSet<string> RotasMigradas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "homeLocador",
                "minhasFerramentas",
                "gerenciarLocacoes",
                "historicoLocacoes",
                // Permite que o item Avaliações do AppHeader navegue para a tela recém-migrada.
                "avaliacao",
                "perfil",
                "cadastroFerramenta",
                "ferramentaDetalhe"
            };

        private readonly IAuthSessionService _authSession;
        private readonly ICatalogoService _catalogo;

        // Ferramentas do locador autenticado (sem filtro de aba aplicado).
        private IReadOnlyList<Produto> _minhasFerramentas = Array.Empty<Produto>();

        // Aba selecionada: null = "Todas".
        private StatusFerramenta? _filtro;

        public MinhasFerramentasViewModel(
            IAuthSessionService authSession,
            ICatalogoService catalogo)
        {
            _authSession = authSession;
            _catalogo = catalogo;

            NavegarCommand =
                new AsyncRelayCommand(
                    async parametro =>
                        await NavegarAsync(parametro as string));

            SelecionarAbaCommand =
                new RelayCommand(
                    parametro =>
                        SelecionarAba(parametro as string));

            VerFerramentaCommand =
                new AsyncRelayCommand(
                    async parametro =>
                        await AbrirFerramentaAsync(
                            parametro,
                            "ferramentaDetalhe"));

            EditarFerramentaCommand =
                new AsyncRelayCommand(
                    async parametro =>
                        await AbrirFerramentaAsync(
                            parametro,
                            "cadastroFerramenta"));

            CadastrarFerramentaCommand =
                new AsyncRelayCommand(
                    async () =>
                    {
                        // null = cadastro de uma ferramenta nova (mesmo comportamento do React).
                        _catalogo.FerramentaSelecionadaId = null;

                        await NavegarAsync(
                            "cadastroFerramenta");
                    });
        }

        // ===================== COMMANDS =====================

        public ICommand NavegarCommand { get; }

        public ICommand SelecionarAbaCommand { get; }

        public ICommand VerFerramentaCommand { get; }

        public ICommand EditarFerramentaCommand { get; }

        public ICommand CadastrarFerramentaCommand { get; }

        // ===================== HEADER =====================

        private string _nomeUsuario = string.Empty;

        public string NomeUsuario
        {
            get => _nomeUsuario;
            private set => SetProperty(
                ref _nomeUsuario,
                value);
        }

        private ImageSource? _fotoUsuario;

        public ImageSource? FotoUsuario
        {
            get => _fotoUsuario;
            private set => SetProperty(
                ref _fotoUsuario,
                value);
        }

        // ===================== ABAS =====================

        private IReadOnlyList<AbaItem> _abas = Array.Empty<AbaItem>();

        public IReadOnlyList<AbaItem> Abas
        {
            get => _abas;
            private set => SetProperty(
                ref _abas,
                value);
        }

        // ===================== LISTA =====================

        // A lista é SUBSTITUÍDA a cada mudança de filtro (e não limpa/preenchida item a item),
        // para a grade redesenhar uma única vez.
        private IReadOnlyList<ProdutoHome> _ferramentas = Array.Empty<ProdutoHome>();

        public IReadOnlyList<ProdutoHome> Ferramentas
        {
            get => _ferramentas;
            private set => SetProperty(
                ref _ferramentas,
                value);
        }

        private bool _temFerramentas;

        public bool TemFerramentas
        {
            get => _temFerramentas;

            private set
            {
                if (SetProperty(
                    ref _temFerramentas,
                    value))
                {
                    OnPropertyChanged(
                        nameof(SemFerramentas));
                }
            }
        }

        public bool SemFerramentas =>
            !TemFerramentas;

        private string _estadoVazioTitulo = string.Empty;

        public string EstadoVazioTitulo
        {
            get => _estadoVazioTitulo;
            private set => SetProperty(
                ref _estadoVazioTitulo,
                value);
        }

        private string _estadoVazioDescricao = string.Empty;

        public string EstadoVazioDescricao
        {
            get => _estadoVazioDescricao;
            private set => SetProperty(
                ref _estadoVazioDescricao,
                value);
        }

        // ===================== CICLO DE VIDA =====================

        /// <summary>
        /// Equivalente a useExigirPerfil(navigate, 'locador', ...):
        /// bloqueia visitante e usuário de outro tipo.
        /// </summary>
        public async Task<bool> GarantirAcessoAsync()
        {
            var usuario =
                _authSession.UsuarioAtual;

            if (usuario is not null &&
                usuario.Tipo == TipoUsuario.Locador)
            {
                return true;
            }

            await Shell.Current.GoToAsync(
                "//login");

            return false;
        }

        /// <summary>
        /// Recarrega as ferramentas do locador autenticado e recalcula abas e lista.
        /// </summary>
        public void Carregar()
        {
            var usuario =
                _authSession.UsuarioAtual;

            if (usuario is null)
                return;

            NomeUsuario =
                usuario.Nome;

            FotoUsuario =
                AvatarHelper.CriarImageSource(usuario.FotoUrl);

            // Somente as ferramentas do locador logado — sempre pelo identificador único
            // (Usuario.LocadorId <-> Produto.LocadorId), nunca pelo nome exibido.
            _minhasFerramentas =
                _catalogo.ObterPorLocador(
                    usuario.LocadorId);

            AtualizarTela();
        }

        // ===================== FILTRO =====================

        private void SelecionarAba(
            string? chave)
        {
            StatusFerramenta? novoFiltro = null;

            if (!string.IsNullOrWhiteSpace(chave) &&
                !string.Equals(
                    chave,
                    ChaveTodas,
                    StringComparison.OrdinalIgnoreCase) &&
                Enum.TryParse<StatusFerramenta>(
                    chave,
                    out var status))
            {
                novoFiltro =
                    status;
            }

            if (_filtro == novoFiltro)
                return;

            _filtro =
                novoFiltro;

            AtualizarTela();
        }

        /// <summary>
        /// Recalcula contagens, abas, lista filtrada e texto do estado vazio
        /// (equivalente aos useMemo de contagem/ferramentasFiltradas no React).
        /// </summary>
        private void AtualizarTela()
        {
            AtualizarAbas();

            var filtradas =
                _filtro is null
                    ? _minhasFerramentas
                    : _minhasFerramentas
                        .Where(
                            p =>
                                p.Status == _filtro)
                        .ToList();

            Ferramentas =
                filtradas
                    .Select(ProdutoHome.DeProduto)
                    .ToList();

            TemFerramentas =
                Ferramentas.Count > 0;

            var (titulo, descricao) =
                ObterTextoEstadoVazio(
                    _filtro);

            EstadoVazioTitulo =
                titulo;

            EstadoVazioDescricao =
                descricao;
        }

        private void AtualizarAbas()
        {
            var abas =
                new List<AbaItem>
                {
                    new(
                        ChaveTodas,
                        "Todas",
                        _minhasFerramentas.Count,
                        _filtro is null)
                };

            foreach (var status in OrdemAbas)
            {
                // Texto da aba vem da configuração central de status (StatusFerramentaConfig).
                var config =
                    StatusFerramentaConfig.Obter(
                        status);

                abas.Add(
                    new AbaItem(
                        status.ToString(),
                        config.TabLabel,
                        _minhasFerramentas.Count(
                            p =>
                                p.Status == status),
                        _filtro == status));
            }

            Abas =
                abas;
        }

        // Textos do estado vazio — ESTADO_VAZIO_TEXTO de MinhasFerramentas.tsx.
        private static (string Titulo, string Descricao) ObterTextoEstadoVazio(
            StatusFerramenta? filtro)
        {
            return filtro switch
            {
                StatusFerramenta.Disponivel =>
                    (
                        "Nenhuma ferramenta disponível",
                        "Ferramentas com o status \"Disponível\" aparecerão aqui."
                    ),

                StatusFerramenta.Locada =>
                    (
                        "Nenhuma ferramenta locada no momento",
                        "Ferramentas em locação no momento aparecerão aqui."
                    ),

                StatusFerramenta.Indisponivel =>
                    (
                        "Nenhuma ferramenta indisponível",
                        "Ferramentas pausadas ou marcadas como indisponíveis aparecerão aqui."
                    ),

                StatusFerramenta.Manutencao =>
                    (
                        "Nenhuma ferramenta em manutenção",
                        "Ferramentas em manutenção aparecerão aqui."
                    ),

                _ =>
                    (
                        "Você ainda não anunciou nenhuma ferramenta",
                        "Clique em \"Cadastrar Ferramenta\" para publicar seu primeiro anúncio."
                    )
            };
        }

        // ===================== NAVEGAÇÃO =====================

        // handleVer / handleEditar do React: guarda a ferramenta escolhida e navega.
        private async Task AbrirFerramentaAsync(
            object? parametro,
            string rota)
        {
            if (parametro is int id)
            {
                _catalogo.FerramentaSelecionadaId =
                    id;
            }

            await NavegarAsync(
                rota);
        }

        private static async Task NavegarAsync(
            string? rota)
        {
            if (string.IsNullOrWhiteSpace(rota) ||
                !RotasMigradas.Contains(rota))
            {
                return;
            }

            // Tocar no item já ativo não deve empilhar a mesma página novamente.
            if (Shell.Current
                .CurrentState
                .Location
                .OriginalString
                .EndsWith(
                    rota,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // "homeLocador" é rota raiz declarada no AppShell.xaml (navegação absoluta),
            // então voltar para o Início desempilha esta página em vez de empilhar outra Home.
            if (string.Equals(
                rota,
                "homeLocador",
                StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.GoToAsync(
                    "//homeLocador");

                return;
            }

            await Shell.Current.GoToAsync(
                rota);
        }
    }
}
