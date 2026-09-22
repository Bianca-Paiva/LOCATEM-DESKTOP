using LOCATEM_DESKTOP.ViewModels.Home;
using MauiIcons;
using MauiIcons.Core;

namespace LOCATEM_DESKTOP.Views.Home
{
    /// <summary>
    /// Home principal do locador.
    /// </summary>
    public partial class HomeLocadorPage : ContentPage
    {
        private const double LarguraMinimaDuasColunas = 1100;

        private readonly HomeLocadorViewModel _viewModel;

        private bool? _painelEmDuasColunas;

        public HomeLocadorPage(
            HomeLocadorViewModel viewModel)
        {
            InitializeComponent();

            _ = new MauiIcon();

            _viewModel = viewModel;

            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (await _viewModel.GarantirAcessoAsync())
            {
                _viewModel.Carregar();
            }

            AtualizarLayoutPainelOperacional();
        }

        /// <summary>
        /// Reorganiza Solicitações Recentes e Agenda da Semana
        /// conforme o espaço REAL disponível no container.
        ///
        /// Tela larga:
        /// [Solicitações] [Agenda]
        ///
        /// Tela menor:
        /// [Solicitações]
        /// [Agenda]
        /// </summary>
        private void PainelOperacionalGrid_SizeChanged(
            object? sender,
            EventArgs e)
        {
            AtualizarLayoutPainelOperacional();
        }

        private void AtualizarLayoutPainelOperacional()
        {
            if (PainelOperacionalGrid.Width <= 0)
            {
                return;
            }

            var duasColunas =
                PainelOperacionalGrid.Width
                >= LarguraMinimaDuasColunas;

            // Evita reposicionar os controles toda vez que a janela mudar apenas alguns pixels.
            if (_painelEmDuasColunas == duasColunas)
            {
                return;
            }

            _painelEmDuasColunas = duasColunas;

            if (duasColunas)
            {
                AplicarLayoutDuasColunas();
            }
            else
            {
                AplicarLayoutUmaColuna();
            }
        }

        /// <summary>
        /// Desktop largo:
        ///
        /// ┌────────────────┐ ┌────────────────┐
        /// │ Solicitações   │ │ Agenda         │
        /// └────────────────┘ └────────────────┘
        /// </summary>
        private void AplicarLayoutDuasColunas()
        {
            // Solicitações
            Grid.SetRow(
                SolicitacoesRecentesCard,
                0);

            Grid.SetColumn(
                SolicitacoesRecentesCard,
                0);

            Grid.SetColumnSpan(
                SolicitacoesRecentesCard,
                1);

            // Agenda
            Grid.SetRow(
                AgendaSemanaCard,
                0);

            Grid.SetColumn(
                AgendaSemanaCard,
                1);

            Grid.SetColumnSpan(
                AgendaSemanaCard,
                1);
        }

        /// <summary>
        /// Desktop menor:
        ///
        /// ┌─────────────────────────────┐
        /// │ Solicitações                │
        /// └─────────────────────────────┘
        ///
        /// ┌─────────────────────────────┐
        /// │ Agenda                      │
        /// └─────────────────────────────┘
        /// </summary>
        private void AplicarLayoutUmaColuna()
        {
            // Solicitações
            Grid.SetRow(
                SolicitacoesRecentesCard,
                0);

            Grid.SetColumn(
                SolicitacoesRecentesCard,
                0);

            Grid.SetColumnSpan(
                SolicitacoesRecentesCard,
                2);

            // Agenda
            Grid.SetRow(
                AgendaSemanaCard,
                1);

            Grid.SetColumn(
                AgendaSemanaCard,
                0);

            Grid.SetColumnSpan(
                AgendaSemanaCard,
                2);
        }
    }
}