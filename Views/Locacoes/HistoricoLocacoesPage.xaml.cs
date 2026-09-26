using LOCATEM_DESKTOP.ViewModels.Locacoes;

namespace LOCATEM_DESKTOP.Views.Locacoes
{
    /// <summary>Tela de histórico das locações encerradas do locador.</summary>
    public partial class HistoricoLocacoesPage : ContentPage
    {
        private const double LarguraMaximaConteudo = 2000;
        private readonly HistoricoLocacoesViewModel _viewModel;
        private bool? _conteudoLimitado;
        private int _ultimaFaixaLargura = -1;

        public HistoricoLocacoesPage(HistoricoLocacoesViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = viewModel;

            SizeChanged += (_, _) => AtualizarResponsividade();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (await _viewModel.GarantirAcessoAsync())
                _viewModel.Carregar();

            AtualizarResponsividade();
        }

        // Replica os três níveis de padding usados no Web sem fixar a largura da janela.
        private void AtualizarResponsividade()
        {
            if (Width <= 0)
                return;

            var limitar = Width > LarguraMaximaConteudo;
            if (_conteudoLimitado != limitar)
            {
                _conteudoLimitado = limitar;
                ConteudoPagina.HorizontalOptions = limitar ? LayoutOptions.Center : LayoutOptions.Fill;
                ConteudoPagina.WidthRequest = limitar ? LarguraMaximaConteudo : -1;
            }

            var faixa = Width >= 1024 ? 2 : Width >= 720 ? 1 : 0;
            if (_ultimaFaixaLargura == faixa)
                return;

            _ultimaFaixaLargura = faixa;
            ConteudoPagina.Padding = faixa switch
            {
                2 => new Thickness(32, 32, 32, 64),
                1 => new Thickness(24, 28, 24, 56),
                _ => new Thickness(16, 20, 16, 48)
            };
        }
    }
}
