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

        // Propriedades opcionais: outras páginas preservam a aparência anterior.
        public static readonly BindableProperty TamanhoTituloProperty =
            BindableProperty.Create(nameof(TamanhoTitulo), typeof(double), typeof(CabecalhoPagina), 28d);
        public double TamanhoTitulo
        {
            get => (double)GetValue(TamanhoTituloProperty);
            set => SetValue(TamanhoTituloProperty, value);
        }

        public static readonly BindableProperty FonteTituloProperty =
            BindableProperty.Create(nameof(FonteTitulo), typeof(string), typeof(CabecalhoPagina), default(string));
        public string? FonteTitulo
        {
            get => (string?)GetValue(FonteTituloProperty);
            set => SetValue(FonteTituloProperty, value);
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

        // Conteúdo opcional à direita do título, na mesma linha (ex.: botão de ação da página).
        // Equivalente à prop "acao" do CabecalhoPagina.tsx. Sem valor, nada é exibido.
        public static readonly BindableProperty AcaoProperty =
            BindableProperty.Create(nameof(Acao), typeof(View), typeof(CabecalhoPagina), null, propertyChanged: OnAcaoChanged);
        public View? Acao { get => (View?)GetValue(AcaoProperty); set => SetValue(AcaoProperty, value); }

        private static void OnAcaoChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var cabecalho = (CabecalhoPagina)bindable;
            var acao = newValue as View;

            cabecalho.AcaoHost.Content = acao;
            cabecalho.AcaoHost.IsVisible = acao is not null;

            // Respiro vertical só quando há ação, para não alterar a altura do cabeçalho sem ela;
            // quando o FlexLayout quebra a linha, é também o espaço entre o título e a ação.
            cabecalho.AcaoHost.Margin = acao is null ? Thickness.Zero : new Thickness(0, 4);
        }
    }
}
