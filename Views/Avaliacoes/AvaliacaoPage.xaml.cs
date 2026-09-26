using LOCATEM_DESKTOP.ViewModels.Avaliacoes;

namespace LOCATEM_DESKTOP.Views.Avaliacoes
{
    /// <summary>
    /// Página de avaliações do locador.
    /// O code-behind cuida somente do ciclo de vida e da largura responsiva; regras ficam no ViewModel.
    /// </summary>
    public partial class AvaliacaoPage : ContentPage
    {
        // Limite visual evita conteúdo excessivamente aberto em monitores muito largos.
        private const double LarguraMaximaConteudo = 1350;

        // Referência ao ViewModel injetado pelo container do MAUI.
        private readonly AvaliacaoViewModel _viewModel;

        // Cache reduz atualizações repetidas de layout durante pequenos redimensionamentos.
        private bool? _conteudoLimitado;
        private int _ultimaFaixaLargura = -1;

        public AvaliacaoPage(AvaliacaoViewModel viewModel)
        {
            InitializeComponent();

            // O BindingContext único alimenta página, cards e modal.
            _viewModel = viewModel;
            BindingContext = viewModel;

            // Responsividade reage ao tamanho real da janela Desktop.
            SizeChanged += (_, _) => AtualizarResponsividade();
        }

        /// <summary>Valida a sessão e recarrega as avaliações sempre que a página volta a ficar visível.</summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (await _viewModel.GarantirAcessoAsync())
                _viewModel.Carregar();

            AtualizarResponsividade();
        }

        // Ajusta largura máxima e padding em três faixas, sem fixar resolução específica.
        private void AtualizarResponsividade()
        {
            if (Width <= 0)
                return;

            // Centraliza o conteúdo em monitores maiores que o limite visual escolhido.
            var limitar = Width > LarguraMaximaConteudo;
            if (_conteudoLimitado != limitar)
            {
                _conteudoLimitado = limitar;
                ConteudoPagina.HorizontalOptions = limitar ? LayoutOptions.Center : LayoutOptions.Fill;
                ConteudoPagina.WidthRequest = limitar ? LarguraMaximaConteudo : -1;
            }

            // Troca apenas quando o breakpoint muda para evitar trabalho desnecessário.
            var faixa = Width >= 1024 ? 2 : Width >= 720 ? 1 : 0;
            if (_ultimaFaixaLargura == faixa)
                return;

            _ultimaFaixaLargura = faixa;
            ConteudoPagina.Padding = faixa switch
            {
                2 => new Thickness(32, 24, 32, 64),
                1 => new Thickness(24, 24, 24, 56),
                _ => new Thickness(16, 18, 16, 48)
            };
        }
    }
}
