using LOCATEM_DESKTOP.Validation.Auth;

namespace LOCATEM_DESKTOP.Components.Auth
{
    /// <summary>Migrado de components/Auth/PasswordMedidor/PasswordStrengthMeter.tsx.</summary>
    public partial class PasswordStrengthMeter : ContentView
    {
        public PasswordStrengthMeter()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty StrengthProperty =
            BindableProperty.Create(nameof(Strength), typeof(PasswordStrength), typeof(PasswordStrengthMeter), PasswordStrength.Nenhuma, propertyChanged: OnStrengthChanged);
        public PasswordStrength Strength { get => (PasswordStrength)GetValue(StrengthProperty); set => SetValue(StrengthProperty, value); }

        // Nome "Visivel" (em vez de IsVisible) para não colidir com o VisualElement.IsVisible nativo.
        public static readonly BindableProperty VisivelProperty =
            BindableProperty.Create(nameof(Visivel), typeof(bool), typeof(PasswordStrengthMeter), false);
        public bool Visivel { get => (bool)GetValue(VisivelProperty); set => SetValue(VisivelProperty, value); }

        public static readonly BindableProperty TextoForcaProperty =
            BindableProperty.Create(nameof(TextoForca), typeof(string), typeof(PasswordStrengthMeter), "Fraca");
        public string TextoForca { get => (string)GetValue(TextoForcaProperty); private set => SetValue(TextoForcaProperty, value); }

        private static void OnStrengthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (PasswordStrengthMeter)bindable;
            var strength = (PasswordStrength)newValue;

            var idle = (Color)Application.Current!.Resources["PasswordBarIdleColor"];
            var fraca = (Color)Application.Current!.Resources["PasswordWeakColor"];
            var media = (Color)Application.Current!.Resources["PasswordMediumColor"];
            var forte = (Color)Application.Current!.Resources["PasswordStrongColor"];

            var corAtiva = strength switch
            {
                PasswordStrength.Fraca => fraca,
                PasswordStrength.Media => media,
                PasswordStrength.Forte => forte,
                _ => idle
            };

            view.Barra1.Color = strength is PasswordStrength.Fraca or PasswordStrength.Media or PasswordStrength.Forte ? corAtiva : idle;
            view.Barra2.Color = strength is PasswordStrength.Media or PasswordStrength.Forte ? corAtiva : idle;
            view.Barra3.Color = strength is PasswordStrength.Forte ? corAtiva : idle;

            view.TextoForca = strength switch
            {
                PasswordStrength.Media => "Média",
                PasswordStrength.Forte => "Forte",
                _ => "Fraca"
            };
        }
    }
}
