using System.Windows.Input;

namespace LOCATEM_DESKTOP.Components.Auth.RecuperarSenha
{
    /// <summary>Migrado de components/Auth/RecuperarSenha/FooterLink/FooterLink.tsx.</summary>
    public partial class FooterLink : ContentView
    {
        public FooterLink()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(FooterLink), string.Empty);
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly BindableProperty LinkTextProperty =
            BindableProperty.Create(nameof(LinkText), typeof(string), typeof(FooterLink), string.Empty);
        public string LinkText { get => (string)GetValue(LinkTextProperty); set => SetValue(LinkTextProperty, value); }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FooterLink));
        public ICommand? Command { get => (ICommand?)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        private void OnLinkTapped(object? sender, TappedEventArgs e) => Command?.Execute(null);
    }
}
