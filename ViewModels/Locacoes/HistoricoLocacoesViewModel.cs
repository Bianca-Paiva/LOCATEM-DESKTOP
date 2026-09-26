using System.Windows.Input;
using LOCATEM_DESKTOP.Components.Shared;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Conta;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Models.Locacoes;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Locacoes;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Locacoes
{
    /// <summary>Estado, filtros e navegação do Histórico de Locações do locador.</summary>
    public class HistoricoLocacoesViewModel : BaseViewModel
    {
        private const string FiltroTodas = "todas";

        private static readonly (string Chave, string Label, StatusLocacao? Status)[] DefinicaoAbas =
        {
            (FiltroTodas, "Todas", null),
            ("finalizada", "Finalizadas", StatusLocacao.Finalizada),
            ("recusada", "Recusadas", StatusLocacao.Recusada),
            ("cancelada", "Canceladas", StatusLocacao.Cancelada)
        };

        private static readonly HashSet<string> RotasMigradas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "homeLocador",
                "minhasFerramentas",
                "gerenciarLocacoes",
                "historicoLocacoes",
                "perfil",
                "cadastroFerramenta",
                "ferramentaDetalhe"
            };

        private readonly IAuthSessionService _authSession;
        private readonly ILocacaoService _locacaoService;
        private IReadOnlyList<Locacao> _historicoCompleto = Array.Empty<Locacao>();
        private string _filtro = FiltroTodas;

        public HistoricoLocacoesViewModel(
            IAuthSessionService authSession,
            ILocacaoService locacaoService)
        {
            _authSession = authSession;
            _locacaoService = locacaoService;

            NavegarCommand = new AsyncRelayCommand(async parametro =>
                await NavegarAsync(parametro as string));

            SelecionarAbaCommand = new RelayCommand(parametro =>
                SelecionarAba(parametro as string));
        }

        public ICommand NavegarCommand { get; }
        public ICommand SelecionarAbaCommand { get; }

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

            // Atualiza cancelamentos automáticos antes de montar o histórico.
            _locacaoService.CancelarPagamentosVencidos();

            _historicoCompleto = _locacaoService
                .ObterPorLocador(usuario.LocadorId)
                .Where(l => l.Status is StatusLocacao.Finalizada or StatusLocacao.Recusada or StatusLocacao.Cancelada)
                .ToList();

            AtualizarTela();
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
            Abas = DefinicaoAbas
                .Select(aba => new AbaItem(
                    aba.Chave,
                    aba.Label,
                    aba.Status is null
                        ? _historicoCompleto.Count
                        : _historicoCompleto.Count(l => l.Status == aba.Status),
                    aba.Chave == _filtro))
                .ToList();

            var filtroAtual = DefinicaoAbas.First(a => a.Chave == _filtro);
            Locacoes = filtroAtual.Status is null
                ? _historicoCompleto.ToList()
                : _historicoCompleto.Where(l => l.Status == filtroAtual.Status).ToList();

            TemLocacoes = Locacoes.Count > 0;
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
