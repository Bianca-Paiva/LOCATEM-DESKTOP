using LOCATEM_DESKTOP.ViewModels.Ferramentas;
using MauiIcons.Core;
using Microsoft.Maui.ApplicationModel;

namespace LOCATEM_DESKTOP.Views.Ferramentas;

public partial class CadastroFerramentaPage : ContentPage
{
    private readonly CadastroFerramentaViewModel _viewModel;

    public CadastroFerramentaPage(CadastroFerramentaViewModel viewModel)
    {
        InitializeComponent();
        // Inicializa os ícones da página e conecta os controles aos dados do formulário.
        _ = new MauiIcon();
        BindingContext = _viewModel = viewModel;
        // Quando a validação falha, mostra a mensagem de erro no início da página.
        _viewModel.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(CadastroFerramentaViewModel.Erro) && _viewModel.TemErro)
                await PaginaScroll.ScrollToAsync(0, 0, true);
        };
        SizeChanged += (_, _) => AjustarColunas();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Carrega o cadastro ou a ferramenta em edição antes de ajustar o layout.
        await _viewModel.CarregarAsync();
        AjustarColunas();
    }

    private void AjustarColunas()
    {
        if (Width <= 0) return;
        // Reproduz os breakpoints do título e da grade usados no Web.
        CabecalhoCadastro.TamanhoTitulo = Width < 640 ? 22 : Width < 1024 ? 26 : 28;
        var estreito = Width < 1024;
        Colunas.ColumnDefinitions[0].Width = new GridLength(1.3, GridUnitType.Star);
        Colunas.ColumnDefinitions[1].Width = estreito ? new GridLength(0) : new GridLength(0.7, GridUnitType.Star);
        Grid.SetColumn(ColunaDireita, estreito ? 0 : 1);
        Grid.SetRow(ColunaDireita, estreito ? 1 : 0);
        Colunas.ColumnSpacing = estreito ? 0 : 24;
        // Em janelas estreitas, as ações ficam empilhadas com publicar primeiro.
        RodapeAcoes.ColumnDefinitions[1].Width = Width < 640 ? new GridLength(0) : GridLength.Star;
        Grid.SetColumn(BotaoPublicar, Width < 640 ? 0 : 1);
        Grid.SetRow(BotaoPublicar, 0);
        Grid.SetRow(BotaoCancelar, Width < 640 ? 1 : 0);
    }

    // Abre a consulta de CEP dos Correios no navegador padrão.
    private async void BuscarCep_Tapped(object? sender, TappedEventArgs e)
    {
        const string url =
            "https://buscacepinter.correios.com.br/app/endereco/index.php";

        try
        {
            var aberto = await Browser.Default.OpenAsync(
                new Uri(url),
                BrowserLaunchMode.External);

            if (!aberto)
                await DisplayAlert(
                    "Busca de CEP",
                    "Não foi possível abrir o navegador.",
                    "OK");
        }
        catch (Exception)
        {
            await DisplayAlert(
                "Busca de CEP",
                "Não foi possível abrir o navegador.",
                "OK");
        }
    }

    // Os botões da quantidade usam o limite de 1 a 999 definido no ViewModel.
    private void QuantidadeMenos_Clicked(object? sender, EventArgs e) =>
        _viewModel.QuantidadeDisponivel--;

    private void QuantidadeMais_Clicked(object? sender, EventArgs e) =>
        _viewModel.QuantidadeDisponivel++;

    // O Enter adiciona o texto ao conjunto de acessórios sem criar um botão extra.
    private void Acessorio_Completed(object? sender, EventArgs e) =>
        _viewModel.AdicionarAcessorioCommand.Execute(null);
}
