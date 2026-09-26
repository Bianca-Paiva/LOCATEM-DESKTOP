using System.Windows.Input;

namespace LOCATEM_DESKTOP.Components.Avaliacoes
{
    /// <summary>
    /// Card visual das avaliações pendentes e realizadas.
    /// Recebe os comandos da página para manter a regra de negócio concentrada no ViewModel.
    /// </summary>
    public partial class CardProdutoAvaliacao : ContentView
    {
        public CardProdutoAvaliacao()
        {
            InitializeComponent();
            SizeChanged += (_, _) => AplicarResponsividade();
        }

        // Command usado pelo toque no card para abrir o modal correspondente.
        public static readonly BindableProperty AbrirCommandProperty = BindableProperty.Create(
            nameof(AbrirCommand),
            typeof(ICommand),
            typeof(CardProdutoAvaliacao));

        public ICommand? AbrirCommand
        {
            get => (ICommand?)GetValue(AbrirCommandProperty);
            set => SetValue(AbrirCommandProperty, value);
        }

        // Command usado pelas estrelas do card quando a avaliação ainda está pendente.
        public static readonly BindableProperty SelecionarNotaCommandProperty = BindableProperty.Create(
            nameof(SelecionarNotaCommand),
            typeof(ICommand),
            typeof(CardProdutoAvaliacao));

        public ICommand? SelecionarNotaCommand
        {
            get => (ICommand?)GetValue(SelecionarNotaCommandProperty);
            set => SetValue(SelecionarNotaCommandProperty, value);
        }

        // Em larguras menores, reduz imagem e espaçamento para evitar corte do conteúdo.
        private void AplicarResponsividade()
        {
            if (Width <= 0)
                return;

            var compacto = Width < 560;
            ConteudoCard.ColumnDefinitions[0].Width = compacto ? 72 : 90;
            ConteudoCard.ColumnSpacing = compacto ? 12 : 20;
        }
    }
}
