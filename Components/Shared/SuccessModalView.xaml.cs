using System.Windows.Input;

namespace LOCATEM_DESKTOP.Components.Shared
{
    /// <summary>
    /// Migrado de components/Shared/SuccessModal/SucessesModal.tsx. Deve ser posicionado como o
    /// último elemento de um Grid raiz (mesma pilha de camadas do Grid/ZIndex), para cobrir a tela
    /// inteira como o overlay "position:fixed" do React.
    /// </summary>
    public partial class SuccessModalView : ContentView
    {
        public SuccessModalView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty IsOpenProperty =
            BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(SuccessModalView), false);
        public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(SuccessModalView), string.Empty);
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly BindableProperty MessageProperty =
            BindableProperty.Create(nameof(Message), typeof(string), typeof(SuccessModalView), string.Empty);
        public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }

        public static readonly BindableProperty ButtonTextProperty =
            BindableProperty.Create(nameof(ButtonText), typeof(string), typeof(SuccessModalView), string.Empty);
        public string ButtonText { get => (string)GetValue(ButtonTextProperty); set => SetValue(ButtonTextProperty, value); }

        public static readonly BindableProperty ConfirmCommandProperty =
            BindableProperty.Create(nameof(ConfirmCommand), typeof(ICommand), typeof(SuccessModalView));
        public ICommand? ConfirmCommand { get => (ICommand?)GetValue(ConfirmCommandProperty); set => SetValue(ConfirmCommandProperty, value); }
    }
}
