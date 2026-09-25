using LOCATEM_DESKTOP.ViewModels.Ferramentas;
using MauiIcons.Core;

namespace LOCATEM_DESKTOP.Views.Ferramentas;

public partial class FerramentaDetalhePage : ContentPage
{
    private readonly FerramentaDetalheViewModel _viewModel;

    public FerramentaDetalhePage(FerramentaDetalheViewModel viewModel)
    {
        InitializeComponent();
        _ = new MauiIcon();
        BindingContext = _viewModel = viewModel;
        SizeChanged += (_, _) => AjustarColunas();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (await _viewModel.CarregarAsync()) AjustarColunas();
    }

    private void AjustarColunas()
    {
        if (Width <= 0) return;

        // FerramentaDetalhe.module.css usa uma coluna antes de 1024px.
        var estreito = Width < 1024;
        Colunas.ColumnDefinitions[0].Width = new GridLength(1.3, GridUnitType.Star);
        Colunas.ColumnDefinitions[1].Width = estreito ? new GridLength(0) : new GridLength(0.7, GridUnitType.Star);
        Grid.SetColumn(ColunaDireita, estreito ? 0 : 1);
        Grid.SetRow(ColunaDireita, estreito ? 1 : 0);
        Colunas.ColumnSpacing = estreito ? 0 : 24;
        Colunas.RowSpacing = estreito ? 20 : 0;
        AreaImagem.HeightRequest = estreito ? 340 : 420;
        CabecalhoDetalhe.TamanhoTitulo = Width < 640 ? 22 : estreito ? 26 : 28;
        Conteudo.Padding = Width < 640 ? new Thickness(16, 20, 16, 56)
            : estreito ? new Thickness(24, 28, 24, 64) : new Thickness(32, 32, 32, 72);
    }
}
