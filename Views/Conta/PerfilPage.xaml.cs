using LOCATEM_DESKTOP.ViewModels.Conta;
using MauiIcons;
using MauiIcons.Core;

namespace LOCATEM_DESKTOP.Views.Conta
{
    public partial class PerfilPage : ContentPage
    {
        private const double LarguraMinimaDuasColunas = 900;
        private const double LarguraMinimaHeaderHorizontal = 620;

        private readonly PerfilViewModel _viewModel;
        private bool? _duasColunas;
        private bool? _headerHorizontal;

        public PerfilPage(PerfilViewModel viewModel)
        {
            InitializeComponent();

            // Garante a inicialização da biblioteca de ícones também nesta página.
            _ = new MauiIcon();

            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (await _viewModel.GarantirAcessoAsync())
                _viewModel.Carregar();

            AtualizarLayoutResponsivo();
        }

        private void ConteudoPerfil_SizeChanged(object? sender, EventArgs e) =>
            AtualizarLayoutResponsivo();

        private void AtualizarLayoutResponsivo()
        {
            if (ConteudoPerfil.Width <= 0)
                return;

            AtualizarColunasInformacoes();
            AtualizarHeaderPerfil();
        }

        private void AtualizarColunasInformacoes()
        {
            var duasColunas = ConteudoPerfil.Width >= LarguraMinimaDuasColunas;
            if (_duasColunas == duasColunas)
                return;

            _duasColunas = duasColunas;

            if (duasColunas)
            {
                Grid.SetRow(InformacoesPessoaisCard, 0);
                Grid.SetColumn(InformacoesPessoaisCard, 0);
                Grid.SetColumnSpan(InformacoesPessoaisCard, 1);

                Grid.SetRow(ReputacaoCard, 0);
                Grid.SetColumn(ReputacaoCard, 1);
                Grid.SetColumnSpan(ReputacaoCard, 1);
            }
            else
            {
                Grid.SetRow(InformacoesPessoaisCard, 0);
                Grid.SetColumn(InformacoesPessoaisCard, 0);
                Grid.SetColumnSpan(InformacoesPessoaisCard, 2);

                Grid.SetRow(ReputacaoCard, 1);
                Grid.SetColumn(ReputacaoCard, 0);
                Grid.SetColumnSpan(ReputacaoCard, 2);
            }
        }

        private void AtualizarHeaderPerfil()
        {
            var horizontal = ConteudoPerfil.Width >= LarguraMinimaHeaderHorizontal;
            if (_headerHorizontal == horizontal)
                return;

            _headerHorizontal = horizontal;

            if (horizontal)
            {
                Grid.SetRow(EditarPerfilBotao, 0);
                Grid.SetColumn(EditarPerfilBotao, 2);
                Grid.SetColumnSpan(EditarPerfilBotao, 1);
                EditarPerfilBotao.HorizontalOptions = LayoutOptions.End;
            }
            else
            {
                Grid.SetRow(EditarPerfilBotao, 1);
                Grid.SetColumn(EditarPerfilBotao, 0);
                Grid.SetColumnSpan(EditarPerfilBotao, 3);
                EditarPerfilBotao.HorizontalOptions = LayoutOptions.Fill;
            }
        }


    }
}
