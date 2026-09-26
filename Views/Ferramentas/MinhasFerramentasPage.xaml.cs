using LOCATEM_DESKTOP.ViewModels.Ferramentas;
using MauiIcons.Core;

namespace LOCATEM_DESKTOP.Views.Ferramentas
{
    /// <summary>
    /// Minhas Ferramentas (Locador) — migrada de
    /// pages/Ferramentas/MinhasFerramentas/MinhasFerramentas.tsx.
    /// </summary>
    public partial class MinhasFerramentasPage : ContentPage
    {
        // .pagina { max-width: 2000px; margin: 0 auto; } do MinhasFerramentas.module.css.
        private const double LarguraMaximaConteudo = 2000;

        private readonly MinhasFerramentasViewModel _viewModel;

        private bool? _conteudoLimitado;

        public MinhasFerramentasPage(
            MinhasFerramentasViewModel viewModel)
        {
            InitializeComponent();

            // Garante a inicialização da biblioteca de ícones também nesta página.
            _ = new MauiIcon();

            _viewModel = viewModel;

            BindingContext = viewModel;

            SizeChanged +=
                (_, _) =>
                    AtualizarLarguraConteudo();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (await _viewModel.GarantirAcessoAsync())
            {
                _viewModel.Carregar();
            }

            AtualizarLarguraConteudo();
        }

        /// <summary>
        /// Em telas até 2000 de largura o conteúdo ocupa toda a janela. Acima disso ele fica
        /// limitado a 2000 e centralizado, como o "max-width + margin: 0 auto" da versão Web.
        /// (A quantidade de colunas dos cards é calculada pela própria GradeResponsiva.)
        /// </summary>
        private void AtualizarLarguraConteudo()
        {
            if (Width <= 0)
            {
                return;
            }

            var limitar =
                Width > LarguraMaximaConteudo;

            if (_conteudoLimitado == limitar)
            {
                return;
            }

            _conteudoLimitado = limitar;

            ConteudoPagina.HorizontalOptions =
                limitar
                    ? LayoutOptions.Center
                    : LayoutOptions.Fill;

            ConteudoPagina.WidthRequest =
                limitar
                    ? LarguraMaximaConteudo
                    : -1;
        }

        // :hover do botão "Cadastrar Ferramenta" (--color-primary-hover).
        private void BotaoCadastrar_PointerEntered(
            object? sender,
            PointerEventArgs e)
        {
            AplicarCorDoBotao(
                sender,
                "AuthPrimaryHoverColor");
        }

        private void BotaoCadastrar_PointerExited(
            object? sender,
            PointerEventArgs e)
        {
            AplicarCorDoBotao(
                sender,
                "AuthPrimaryColor");
        }

        private static void AplicarCorDoBotao(
            object? sender,
            string chaveCor)
        {
            if (sender is Border botao &&
                Application.Current is not null &&
                Application.Current.Resources.TryGetValue(
                    chaveCor,
                    out var valor) &&
                valor is Color cor)
            {
                botao.BackgroundColor = cor;
            }
        }
    }
}
