using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers.Auth;

namespace LOCATEM_DESKTOP.Components.Shared
{
    /// <summary>
    /// Componente reutilizável — migrado de components/Shared/Inputs/FormInput/FormInput.tsx.
    /// Label + campo de texto + mensagem de erro, com borda vermelha (Status=Erro) e animação
    /// de "chacoalhar" (IsShaking) equivalentes ao FormInput original.
    /// </summary>
    public partial class FormEntry : ContentView
    {
        public FormEntry()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty MaxLengthProperty =
            BindableProperty.Create(
                nameof(MaxLength),
                typeof(int),
                typeof(FormEntry),
                0);

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        // Adicione esta propriedade BindableProperty para Label
        public static readonly BindableProperty LabelProperty =
            BindableProperty.Create(
                nameof(Label),
                typeof(string),
                typeof(FormEntry),
                string.Empty);

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(FormEntry), string.Empty, BindingMode.TwoWay);
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FormEntry), string.Empty);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty EntryKeyboardProperty =
            BindableProperty.Create(nameof(EntryKeyboard), typeof(Keyboard), typeof(FormEntry), Keyboard.Default);
        public Keyboard EntryKeyboard { get => (Keyboard)GetValue(EntryKeyboardProperty); set => SetValue(EntryKeyboardProperty, value); }

        public static readonly BindableProperty IsRequiredFieldProperty =
            BindableProperty.Create(nameof(IsRequiredField), typeof(bool), typeof(FormEntry), false);
        public bool IsRequiredField { get => (bool)GetValue(IsRequiredFieldProperty); set => SetValue(IsRequiredFieldProperty, value); }

        public static readonly BindableProperty ErrorTextProperty =
            BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(FormEntry), string.Empty, propertyChanged: OnErrorTextChanged);
        public string ErrorText { get => (string)GetValue(ErrorTextProperty); set => SetValue(ErrorTextProperty, value); }

        public static readonly BindableProperty HasErrorTextProperty =
            BindableProperty.Create(nameof(HasErrorText), typeof(bool), typeof(FormEntry), false);
        public bool HasErrorText { get => (bool)GetValue(HasErrorTextProperty); private set => SetValue(HasErrorTextProperty, value); }

        public static readonly BindableProperty IsShakingProperty =
            BindableProperty.Create(nameof(IsShaking), typeof(bool), typeof(FormEntry), false);
        public bool IsShaking { get => (bool)GetValue(IsShakingProperty); set => SetValue(IsShakingProperty, value); }

        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create(nameof(Status), typeof(FieldStatus), typeof(FormEntry), FieldStatus.Neutro, propertyChanged: OnStatusChanged);
        public FieldStatus Status { get => (FieldStatus)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

        /// <summary>Equivalente a onBlur={() => trigger('campo')} usado no Cadastro.</summary>
        public static readonly BindableProperty UnfocusedCommandProperty =
            BindableProperty.Create(nameof(UnfocusedCommand), typeof(ICommand), typeof(FormEntry));
        public ICommand? UnfocusedCommand { get => (ICommand?)GetValue(UnfocusedCommandProperty); set => SetValue(UnfocusedCommandProperty, value); }

        private static void OnErrorTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((FormEntry)bindable).HasErrorText = !string.IsNullOrEmpty(newValue as string);
        }

        private static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (FormEntry)bindable;
            view.InputBorder.Stroke = (FieldStatus)newValue == FieldStatus.Erro
                ? (Brush)Application.Current!.Resources["ErrorBrush"]
                : new SolidColorBrush((Color)Application.Current!.Resources["InputBorderColor"]);
            view.InputBorder.StrokeThickness = (FieldStatus)newValue == FieldStatus.Erro ? 1.5 : 1.5;
        }

        private static void OnMaxLengthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (FormEntry)bindable;
            // Se o XAML já criou o InnerEntry, aplica o valor; caso contrário será aplicado quando o controle for inicializado
            if (view.InnerEntry is not null)
            {
                view.InnerEntry.MaxLength = (int)newValue;
            }
        }

        // Wiring mínimo de evento -> comando, sem lógica de negócio no code-behind
        // (equivalente ao onBlur={() => trigger('campo')} do React).
        private void OnEntryUnfocused(object? sender, FocusEventArgs e) => UnfocusedCommand?.Execute(null);
    }
}
