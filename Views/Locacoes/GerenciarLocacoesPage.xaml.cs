using LOCATEM_DESKTOP.ViewModels.Locacoes;
using Microsoft.Maui.Dispatching;

namespace LOCATEM_DESKTOP.Views.Locacoes
{
    /// <summary>Gerenciar Locações — visão operacional do locador.</summary>
    public partial class GerenciarLocacoesPage : ContentPage
    {
        private const double LarguraMaximaConteudo = 2000;
        private readonly GerenciarLocacoesViewModel _viewModel;
        private readonly IDispatcherTimer _prazoTimer;
        private bool? _conteudoLimitado;
        private double _ultimaFaixaLargura = -1;

        public GerenciarLocacoesPage(GerenciarLocacoesViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = viewModel;

            _prazoTimer = Dispatcher.CreateTimer();
            _prazoTimer.Interval = TimeSpan.FromMinutes(1);
            _prazoTimer.Tick += (_, _) => _viewModel.VerificarPrazosPagamento();

            SizeChanged += (_, _) => AtualizarResponsividade();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (await _viewModel.GarantirAcessoAsync())
            {
                _viewModel.Carregar();
                if (!_prazoTimer.IsRunning)
                    _prazoTimer.Start();
            }

            AtualizarResponsividade();
        }

        protected override void OnDisappearing()
        {
            _prazoTimer.Stop();
            base.OnDisappearing();
        }

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

            var faixa = Width >= 1024 ? 2 : Width >= 640 ? 1 : 0;
            if (Math.Abs(faixa - _ultimaFaixaLargura) < 0.1)
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
