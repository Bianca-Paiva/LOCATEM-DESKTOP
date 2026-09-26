using LOCATEM_DESKTOP.Components.Locacoes;
using LOCATEM_DESKTOP.Models.Locacoes;

namespace LOCATEM_DESKTOP.Components.Locacoes.Historico
{
    /// <summary>Card expansível usado exclusivamente no histórico do locador.</summary>
    public partial class LocacaoHistoricoCard : ContentView
    {
        private bool _expandido;
        private bool? _layoutCompacto;

        public LocacaoHistoricoCard()
        {
            InitializeComponent();
            SizeChanged += (_, _) => AtualizarLayoutResponsivo();
        }

        public static readonly BindableProperty LocacaoProperty =
            BindableProperty.Create(
                nameof(Locacao),
                typeof(Locacao),
                typeof(LocacaoHistoricoCard),
                propertyChanged: OnLocacaoChanged);

        public Locacao? Locacao
        {
            get => (Locacao?)GetValue(LocacaoProperty);
            set => SetValue(LocacaoProperty, value);
        }

        private static void OnLocacaoChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var card = (LocacaoHistoricoCard)bindable;
            card._expandido = false;
            card.AtualizarConteudo();
        }

        private void OnCardTapped(object? sender, TappedEventArgs e)
        {
            if (Locacao is null)
                return;

            _expandido = !_expandido;
            AtualizarEstadoExpandido();
        }

        private void AtualizarConteudo()
        {
            if (Locacao is null)
            {
                PainelExpandido.IsVisible = false;
                return;
            }

            QuantidadeLabel.Text = Locacao.Quantidade == 1
                ? "1 unidade"
                : $"{Locacao.Quantidade} unidades";

            // Somente uma locação finalizada representa receita no histórico.
            var finalizada = Locacao.Status == StatusLocacao.Finalizada;
            ValorLabel.Text = finalizada ? $"+ {Locacao.Valor}" : "—";
            ValorLabel.TextColor = Color.FromArgb(finalizada ? "#137333" : "#8A9099");

            MotivoLabel.Text = ObterMotivoStatus(Locacao);
            AplicarVisualMotivo(Locacao.Status);
            AtualizarEstadoExpandido();
            AtualizarLayoutResponsivo();
        }

        private void AplicarVisualMotivo(StatusLocacao status)
        {
            var config = StatusLocacaoConfig.Obter(status);
            var cor = Color.FromArgb(config.Cor);

            MotivoIcone.Icon = config.Icone;
            MotivoIcone.IconColor = cor;
            MotivoCirculo.BackgroundColor = Color.FromArgb(config.Fundo);
            MotivoCirculo.Stroke = new SolidColorBrush(Color.FromArgb(config.Borda));
        }

        private static string ObterMotivoStatus(Locacao locacao)
        {
            if (locacao.Status == StatusLocacao.Cancelada &&
                !string.IsNullOrWhiteSpace(locacao.MotivoCancelamento))
            {
                return locacao.MotivoCancelamento;
            }

            if (!string.IsNullOrWhiteSpace(locacao.MensagemStatus))
                return locacao.MensagemStatus;

            return locacao.Status switch
            {
                StatusLocacao.Finalizada => "Locação concluída com sucesso.",
                StatusLocacao.Recusada => "Você recusou esta solicitação de locação.",
                StatusLocacao.Cancelada => "Esta locação foi cancelada.",
                _ => string.Empty
            };
        }

        private void AtualizarEstadoExpandido()
        {
            PainelExpandido.IsVisible = _expandido;
            ExpandirGlyph.Text = _expandido ? "⌃" : "⌄";
            CardBorder.Stroke = new SolidColorBrush(
                Color.FromArgb(_expandido ? "#D9DDE3" : "#E9EAEC"));
        }

        private void AtualizarLayoutResponsivo()
        {
            if (Width <= 0)
                return;

            var compacto = Width < 720;
            if (_layoutCompacto == compacto)
                return;

            _layoutCompacto = compacto;
            if (compacto)
                AplicarLayoutCompacto();
            else
                AplicarLayoutDesktop();
        }

        private void AplicarLayoutDesktop()
        {
            MiniaturaContainer.WidthRequest = 52;
            MiniaturaContainer.HeightRequest = 52;
            BotaoDetalhes.Margin = Thickness.Zero;

            CabecalhoGrid.ColumnDefinitions.Clear();
            CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(52) });
            CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            CabecalhoGrid.RowDefinitions.Clear();
            CabecalhoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Posicionar(MiniaturaContainer, 0, 0);
            Posicionar(NomeFerramentaLabel, 1, 0);
            Posicionar(LocatarioContainer, 2, 0);
            Posicionar(Aside, 3, 0);
            Aside.HorizontalOptions = LayoutOptions.End;
            Aside.Margin = Thickness.Zero;

            DetalhesGrid.ColumnDefinitions.Clear();
            DetalhesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            DetalhesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            DetalhesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            DetalhesGrid.RowDefinitions.Clear();
            DetalhesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Posicionar(PeriodoItem, 0, 0);
            Posicionar(QuantidadeItem, 1, 0);
            Posicionar(MotivoItem, 2, 0);

            RodapeGrid.ColumnDefinitions.Clear();
            RodapeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            RodapeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            RodapeGrid.RowDefinitions.Clear();
            RodapeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Posicionar(DescricaoDetalhes, 0, 0);
            Posicionar(BotaoDetalhes, 1, 0);
        }

        private void AplicarLayoutCompacto()
        {
            CabecalhoGrid.ColumnDefinitions.Clear();
            CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(48) });
            CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            CabecalhoGrid.RowDefinitions.Clear();
            for (var i = 0; i < 3; i++)
                CabecalhoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            MiniaturaContainer.WidthRequest = 48;
            MiniaturaContainer.HeightRequest = 48;
            Posicionar(MiniaturaContainer, 0, 0, rowSpan: 3);
            Posicionar(NomeFerramentaLabel, 1, 0);
            Posicionar(LocatarioContainer, 1, 1);
            Posicionar(Aside, 1, 2);
            Aside.HorizontalOptions = LayoutOptions.Start;
            Aside.Margin = new Thickness(0, 6, 0, 0);

            DetalhesGrid.ColumnDefinitions.Clear();
            DetalhesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            DetalhesGrid.RowDefinitions.Clear();
            for (var i = 0; i < 3; i++)
                DetalhesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Posicionar(PeriodoItem, 0, 0);
            Posicionar(QuantidadeItem, 0, 1);
            Posicionar(MotivoItem, 0, 2);

            RodapeGrid.ColumnDefinitions.Clear();
            RodapeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            RodapeGrid.RowDefinitions.Clear();
            RodapeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RodapeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Posicionar(DescricaoDetalhes, 0, 0);
            Posicionar(BotaoDetalhes, 0, 1);
            BotaoDetalhes.Margin = new Thickness(0, 4, 0, 0);
        }

        private static void Posicionar(View view, int coluna, int linha, int columnSpan = 1, int rowSpan = 1)
        {
            Grid.SetColumn(view, coluna);
            Grid.SetRow(view, linha);
            Grid.SetColumnSpan(view, columnSpan);
            Grid.SetRowSpan(view, rowSpan);
        }
    }
}
