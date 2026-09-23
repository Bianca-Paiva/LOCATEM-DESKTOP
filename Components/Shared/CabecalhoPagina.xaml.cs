namespace LOCATEM_DESKTOP.Components.Shared
{
    // Cabeçalho padrão das páginas internas (título + subtítulo opcional).
    // Migrado de components/Layout/CabecalhoPagina/CabecalhoPagina.tsx.
    // Diferente de PageHeader (usado no fluxo de Auth), este é alinhado à esquerda.
    public partial class CabecalhoPagina : ContentView
    {
        public CabecalhoPagina()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TituloProperty =
            BindableProperty.Create(nameof(Titulo), typeof(string), typeof(CabecalhoPagina), string.Empty);
        public string Titulo { get => (string)GetValue(TituloProperty); set => SetValue(TituloProperty, value); }

        public static readonly BindableProperty SubtituloProperty =
            BindableProperty.Create(nameof(Subtitulo), typeof(string), typeof(CabecalhoPagina), string.Empty, propertyChanged: OnSubtituloChanged);
        public string Subtitulo { get => (string)GetValue(SubtituloProperty); set => SetValue(SubtituloProperty, value); }

        public static readonly BindableProperty HasSubtituloProperty =
            BindableProperty.Create(nameof(HasSubtitulo), typeof(bool), typeof(CabecalhoPagina), false);
        public bool HasSubtitulo { get => (bool)GetValue(HasSubtituloProperty); private set => SetValue(HasSubtituloProperty, value); }

        private static void OnSubtituloChanged(BindableObject bindable, object oldValue, object newValue) =>
            ((CabecalhoPagina)bindable).HasSubtitulo = !string.IsNullOrEmpty(newValue as string);
    }
}
