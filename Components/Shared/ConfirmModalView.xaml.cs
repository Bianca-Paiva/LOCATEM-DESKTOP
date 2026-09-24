using System.Windows.Input;

namespace LOCATEM_DESKTOP.Components.Shared
{
    /// <summary>
    /// Modal de confirmação reutilizável, equivalente ao ConfirmModal do projeto React.
    /// </summary>
    public partial class ConfirmModalView : ContentView
    {
        public ConfirmModalView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty IsOpenProperty =
            BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(ConfirmModalView), false);

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(ConfirmModalView), string.Empty);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty MessageProperty =
            BindableProperty.Create(nameof(Message), typeof(string), typeof(ConfirmModalView), string.Empty);

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly BindableProperty ConfirmTextProperty =
            BindableProperty.Create(nameof(ConfirmText), typeof(string), typeof(ConfirmModalView), "Confirmar");

        public string ConfirmText
        {
            get => (string)GetValue(ConfirmTextProperty);
            set => SetValue(ConfirmTextProperty, value);
        }

        public static readonly BindableProperty CancelTextProperty =
            BindableProperty.Create(nameof(CancelText), typeof(string), typeof(ConfirmModalView), "Cancelar");

        public string CancelText
        {
            get => (string)GetValue(CancelTextProperty);
            set => SetValue(CancelTextProperty, value);
        }

        public static readonly BindableProperty ConfirmCommandProperty =
            BindableProperty.Create(nameof(ConfirmCommand), typeof(ICommand), typeof(ConfirmModalView));

        public ICommand? ConfirmCommand
        {
            get => (ICommand?)GetValue(ConfirmCommandProperty);
            set => SetValue(ConfirmCommandProperty, value);
        }

        public static readonly BindableProperty CancelCommandProperty =
            BindableProperty.Create(nameof(CancelCommand), typeof(ICommand), typeof(ConfirmModalView));

        public ICommand? CancelCommand
        {
            get => (ICommand?)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }
    }
}
