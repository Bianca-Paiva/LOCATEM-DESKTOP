using LOCATEM_DESKTOP.Helpers.Auth;

namespace LOCATEM_DESKTOP.Components.Shared
{
    /// <summary>
    /// Componente reutilizável — migrado de components/Shared/Inputs/PasswordInput/PasswordInput.tsx.
    /// Igual ao FormEntry, mas com botão de mostrar/ocultar senha (ícones Material "Visibility" /
    /// "VisibilityOff" via AathifMahir.Maui.MauiIcons.Material).
    /// </summary>
    public partial class PasswordEntry : ContentView
    {
        public PasswordEntry()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty LabelProperty =
            BindableProperty.Create(nameof(Label), typeof(string), typeof(PasswordEntry), string.Empty);
        public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(PasswordEntry), string.Empty, BindingMode.TwoWay);
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(PasswordEntry), string.Empty);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty IsRequiredFieldProperty =
            BindableProperty.Create(nameof(IsRequiredField), typeof(bool), typeof(PasswordEntry), false);
        public bool IsRequiredField { get => (bool)GetValue(IsRequiredFieldProperty); set => SetValue(IsRequiredFieldProperty, value); }

        public static readonly BindableProperty ErrorTextProperty =
            BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(PasswordEntry), string.Empty, propertyChanged: OnErrorTextChanged);
        public string ErrorText { get => (string)GetValue(ErrorTextProperty); set => SetValue(ErrorTextProperty, value); }

        public static readonly BindableProperty HasErrorTextProperty =
            BindableProperty.Create(nameof(HasErrorText), typeof(bool), typeof(PasswordEntry), false);
        public bool HasErrorText { get => (bool)GetValue(HasErrorTextProperty); private set => SetValue(HasErrorTextProperty, value); }

        public static readonly BindableProperty IsShakingProperty =
            BindableProperty.Create(nameof(IsShaking), typeof(bool), typeof(PasswordEntry), false);
        public bool IsShaking { get => (bool)GetValue(IsShakingProperty); set => SetValue(IsShakingProperty, value); }

        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create(nameof(Status), typeof(FieldStatus), typeof(PasswordEntry), FieldStatus.Neutro, propertyChanged: OnStatusChanged);
        public FieldStatus Status { get => (FieldStatus)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

        public static readonly BindableProperty IsPasswordVisibleProperty =
            BindableProperty.Create(nameof(IsPasswordVisible), typeof(bool), typeof(PasswordEntry), false);
        public bool IsPasswordVisible { get => (bool)GetValue(IsPasswordVisibleProperty); set => SetValue(IsPasswordVisibleProperty, value); }

        /// <summary>Inverso de IsPasswordVisible — passado ao Entry.IsPassword.</summary>
        public bool IsPassword => !IsPasswordVisible;

        // Novo: MaxLength para repassar ao Entry interno (0 = sem limite)
        public static readonly BindableProperty MaxLengthProperty =
            BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(PasswordEntry), int.MaxValue, propertyChanged: OnMaxLengthChanged);
        public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }

        private static void OnErrorTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PasswordEntry)bindable).HasErrorText = !string.IsNullOrEmpty(newValue as string);
        }

        private static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (PasswordEntry)bindable;
            view.InputBorder.Stroke = (FieldStatus)newValue == FieldStatus.Erro
                ? (Brush)Application.Current!.Resources["ErrorBrush"]
                : new SolidColorBrush((Color)Application.Current!.Resources["InputBorderColor"]);
        }

        private static void OnMaxLengthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (PasswordEntry)bindable;
            // Se o XAML já vinculou InnerEntry.MaxLength, isso garante que mudanças via código também reflitam.
            if (view.InnerEntry is not null)
            {
                view.InnerEntry.MaxLength = (int)newValue;
            }
        }

        private void OnToggleVisibilityClicked(object? sender, TappedEventArgs e)
        {
            IsPasswordVisible = !IsPasswordVisible;
            OnPropertyChanged(nameof(IsPassword));
        }
    }
}
