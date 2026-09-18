using System.Windows.Input;
using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Components.Ferramentas
{
    //
    // Card de ferramenta — migrado de components/Ferramentas/ProductCard/ProductCard.tsx.
    // O carrossel de imagens (Swiper) foi reduzido a uma imagem: no catálogo atual todas as
    // posições de `images` de um produto apontam para o mesmo arquivo.
    //
    public partial class ProductCard : ContentView
    {
        public ProductCard()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(ProductCard), string.Empty);
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly BindableProperty BrandProperty =
            BindableProperty.Create(nameof(Brand), typeof(string), typeof(ProductCard), string.Empty);
        public string Brand { get => (string)GetValue(BrandProperty); set => SetValue(BrandProperty, value); }

        public static readonly BindableProperty PriceProperty =
            BindableProperty.Create(nameof(Price), typeof(string), typeof(ProductCard), string.Empty);
        public string Price { get => (string)GetValue(PriceProperty); set => SetValue(PriceProperty, value); }

        public static readonly BindableProperty ImagemProperty =
            BindableProperty.Create(nameof(Imagem), typeof(string), typeof(ProductCard), string.Empty);
        public string Imagem { get => (string)GetValue(ImagemProperty); set => SetValue(ImagemProperty, value); }

        public static readonly BindableProperty RatingProperty =
            BindableProperty.Create(nameof(Rating), typeof(string), typeof(ProductCard), string.Empty);
        public string Rating { get => (string)GetValue(RatingProperty); set => SetValue(RatingProperty, value); }

        public static readonly BindableProperty ReviewCountProperty =
            BindableProperty.Create(nameof(ReviewCount), typeof(int), typeof(ProductCard), 0, propertyChanged: OnReviewCountChanged);
        public int ReviewCount { get => (int)GetValue(ReviewCountProperty); set => SetValue(ReviewCountProperty, value); }

        public static readonly BindableProperty ReviewCountTextoProperty =
            BindableProperty.Create(nameof(ReviewCountTexto), typeof(string), typeof(ProductCard), "(0)");
        public string ReviewCountTexto { get => (string)GetValue(ReviewCountTextoProperty); private set => SetValue(ReviewCountTextoProperty, value); }

        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create(nameof(Status), typeof(StatusFerramenta), typeof(ProductCard), StatusFerramenta.Disponivel);
        public StatusFerramenta Status { get => (StatusFerramenta)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

        public static readonly BindableProperty MostrarAcoesProperty =
            BindableProperty.Create(nameof(MostrarAcoes), typeof(bool), typeof(ProductCard), false);
        public bool MostrarAcoes { get => (bool)GetValue(MostrarAcoesProperty); set => SetValue(MostrarAcoesProperty, value); }

        public static readonly BindableProperty VerCommandProperty =
            BindableProperty.Create(nameof(VerCommand), typeof(ICommand), typeof(ProductCard));
        public ICommand? VerCommand { get => (ICommand?)GetValue(VerCommandProperty); set => SetValue(VerCommandProperty, value); }

        public static readonly BindableProperty EditarCommandProperty =
            BindableProperty.Create(nameof(EditarCommand), typeof(ICommand), typeof(ProductCard));
        public ICommand? EditarCommand { get => (ICommand?)GetValue(EditarCommandProperty); set => SetValue(EditarCommandProperty, value); }

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ProductCard));
        public object? CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        private static void OnReviewCountChanged(BindableObject bindable, object oldValue, object newValue) =>
            ((ProductCard)bindable).ReviewCountTexto = $"({newValue})";
    }
}
