using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers.Locacoes;
using LOCATEM_DESKTOP.Models.Locacoes;
using MauiIcons.Material;

namespace LOCATEM_DESKTOP.Components.Locacoes.Gerenciar
{
    /// <summary>Card operacional do locador, migrado de LocacaoLocadorCard.tsx.</summary>
    public partial class LocacaoLocadorCard : ContentView
    {
        private bool _expandido;
        private double _ultimaLargura = -1;

        public LocacaoLocadorCard()
        {
            InitializeComponent();
            SizeChanged += (_, _) => AtualizarLayoutResponsivo();
        }

        public static readonly BindableProperty LocacaoProperty =
            BindableProperty.Create(
                nameof(Locacao),
                typeof(Locacao),
                typeof(LocacaoLocadorCard),
                propertyChanged: OnLocacaoChanged);

        public Locacao? Locacao
        {
            get => (Locacao?)GetValue(LocacaoProperty);
            set => SetValue(LocacaoProperty, value);
        }

        public static readonly BindableProperty AbrirPendenteCommandProperty =
            BindableProperty.Create(
                nameof(AbrirPendenteCommand),
                typeof(ICommand),
                typeof(LocacaoLocadorCard));

        public ICommand? AbrirPendenteCommand
        {
            get => (ICommand?)GetValue(AbrirPendenteCommandProperty);
            set => SetValue(AbrirPendenteCommandProperty, value);
        }

        private static void OnLocacaoChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var card = (LocacaoLocadorCard)bindable;
            card._expandido = false;
            card.AtualizarConteudo();
        }

        private void OnCardTapped(object? sender, TappedEventArgs e)
        {
            if (Locacao is null)
                return;

            if (Locacao.Status == StatusLocacao.Pendente)
            {
                AbrirPendenteCommand?.Execute(Locacao);
                return;
            }

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

            var acaoAgora = LocacaoGerenciamentoHelper.ObterAcaoAgora(Locacao);
            var proximaEtapa = LocacaoGerenciamentoHelper.ObterProximaEtapa(Locacao);

            AcaoTituloLabel.Text = acaoAgora.Titulo;
            AcaoDescricaoLabel.Text = acaoAgora.Descricao;
            HorarioEntregaLabel.Text = LocacaoGerenciamentoHelper.FormatarHorarioEntrega(Locacao);
            JanelaDevolucaoLabel.Text = LocacaoGerenciamentoHelper.FormatarJanelaDevolucao(Locacao);
            QuantidadeLabel.Text = Locacao.Quantidade == 1
                ? "1 unidade"
                : $"{Locacao.Quantidade} unidades";
            ProximaEtapaTituloLabel.Text = proximaEtapa.Titulo;
            ProximaEtapaDescricaoLabel.Text = proximaEtapa.Descricao;
            AcaoIcone.Icon = ObterIconeAcao(Locacao.Status);

            AtualizarEstadoExpandido();
            AtualizarLayoutResponsivo();
        }

        private void AtualizarEstadoExpandido()
        {
            var ehPendente = Locacao?.Status == StatusLocacao.Pendente;
            var mostrarPainel = _expandido && !ehPendente;

            PainelExpandido.IsVisible = mostrarPainel;
            MensagemStatusLabel.IsVisible = !mostrarPainel;
            CardBorder.Stroke = mostrarPainel
                ? new SolidColorBrush(Color.FromArgb("#D9DDE3"))
                : new SolidColorBrush(Color.FromArgb("#EEEEEE"));

            ExpandirGlyph.Text = ehPendente
                ? "›"
                : mostrarPainel ? "⌃" : "⌄";
        }

        private void AtualizarLayoutResponsivo()
        {
            if (Width <= 0 || Math.Abs(Width - _ultimaLargura) < 1)
                return;

            _ultimaLargura = Width;
            var compacto = Width < 640;

            if (compacto)
            {
                CabecalhoGrid.ColumnDefinitions.Clear();
                CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(64) });
                CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                CabecalhoGrid.RowDefinitions.Clear();
                CabecalhoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                CabecalhoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                MiniaturaContainer.WidthRequest = 64;
                MiniaturaContainer.HeightRequest = 64;
                Grid.SetColumn(MiniaturaContainer, 0);
                Grid.SetRow(MiniaturaContainer, 0);
                Grid.SetColumn(ConteudoPrincipal, 1);
                Grid.SetRow(ConteudoPrincipal, 0);
                Grid.SetColumn(Aside, 0);
                Grid.SetColumnSpan(Aside, 2);
                Grid.SetRow(Aside, 1);
                Aside.HorizontalOptions = LayoutOptions.Fill;
                Aside.Margin = new Thickness(0, 12, 0, 0);
            }
            else
            {
                CabecalhoGrid.ColumnDefinitions.Clear();
                CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
                CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                CabecalhoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                CabecalhoGrid.RowDefinitions.Clear();
                CabecalhoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                MiniaturaContainer.WidthRequest = 90;
                MiniaturaContainer.HeightRequest = 90;
                Grid.SetColumn(MiniaturaContainer, 0);
                Grid.SetRow(MiniaturaContainer, 0);
                Grid.SetColumn(ConteudoPrincipal, 1);
                Grid.SetRow(ConteudoPrincipal, 0);
                Grid.SetColumn(Aside, 2);
                Grid.SetColumnSpan(Aside, 1);
                Grid.SetRow(Aside, 0);
                Aside.HorizontalOptions = LayoutOptions.End;
                Aside.Margin = Thickness.Zero;
            }

            AtualizarGradeInformacoes();
        }

        private void AtualizarGradeInformacoes()
        {
            InfoGrade.ColumnDefinitions.Clear();
            InfoGrade.RowDefinitions.Clear();

            var colunas = Width >= 1024 ? 4 : Width >= 640 ? 2 : 1;
            var linhas = (int)Math.Ceiling(4d / colunas);

            for (var i = 0; i < colunas; i++)
                InfoGrade.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            for (var i = 0; i < linhas; i++)
                InfoGrade.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var itens = new View[] { InfoEntrega, InfoDevolucao, InfoQuantidade, InfoProximaEtapa };
            for (var i = 0; i < itens.Length; i++)
            {
                Grid.SetColumn(itens[i], i % colunas);
                Grid.SetRow(itens[i], i / colunas);
            }
        }

        private static MaterialIcons ObterIconeAcao(StatusLocacao status) => status switch
        {
            StatusLocacao.Confirmada or StatusLocacao.PreparandoEntrega => MaterialIcons.Inventory,
            StatusLocacao.EmTransporte or StatusLocacao.DevolucaoEmTransporte => MaterialIcons.LocalShipping,
            StatusLocacao.AguardandoDevolucao => MaterialIcons.AssignmentReturn,
            StatusLocacao.AguardandoPagamento => MaterialIcons.Schedule,
            StatusLocacao.EmAndamento or StatusLocacao.Finalizada or StatusLocacao.Recusada or StatusLocacao.Cancelada => MaterialIcons.CheckCircle,
            _ => MaterialIcons.Schedule
        };
    }
}
