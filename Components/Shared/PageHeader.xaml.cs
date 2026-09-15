namespace LOCATEM_DESKTOP.Components.Shared
{
    /// <summary>Migrado de components/Auth/RecuperarSenha/PageHeader/PageHeader.tsx.</summary>
    public partial class PageHeader : ContentView
    {
        public PageHeader()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(PageHeader), string.Empty);
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly BindableProperty SubtitleProperty =
            BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(PageHeader), string.Empty, propertyChanged: OnSubtitleChanged);
        public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }

        public static readonly BindableProperty HasSubtitleProperty =
            BindableProperty.Create(nameof(HasSubtitle), typeof(bool), typeof(PageHeader), false);
        public bool HasSubtitle { get => (bool)GetValue(HasSubtitleProperty); private set => SetValue(HasSubtitleProperty, value); }

        private static void OnSubtitleChanged(BindableObject bindable, object oldValue, object newValue) =>
            ((PageHeader)bindable).HasSubtitle = !string.IsNullOrEmpty(newValue as string);
    }
}
