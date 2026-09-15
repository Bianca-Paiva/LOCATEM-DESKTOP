namespace LOCATEM_DESKTOP.Components.Auth.RecuperarSenha
{
    /// <summary>Migrado de components/Auth/RecuperarSenha/Etapas/Etapas.tsx.</summary>
    public partial class RecoveryStepper : ContentView
    {
        public RecoveryStepper()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CurrentStepProperty =
            BindableProperty.Create(nameof(CurrentStep), typeof(int), typeof(RecoveryStepper), 1, propertyChanged: OnCurrentStepChanged);
        public int CurrentStep { get => (int)GetValue(CurrentStepProperty); set => SetValue(CurrentStepProperty, value); }

        private static void OnCurrentStepChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (RecoveryStepper)bindable;
            var atual = (int)newValue;

            var neutro = (Color)Application.Current!.Resources["TextSecondaryMutedColor"];
            var ativo = (Color)Application.Current!.Resources["StepActiveColor"];
            var borderNeutro = (Color)Application.Current!.Resources["StepBorderColor"];

            void Aplicar(Border circulo, Label numero, Label label, Microsoft.Maui.Controls.View check, int passo)
            {
                var completo = passo < atual;
                var atualPasso = passo == atual;

                circulo.Stroke = new SolidColorBrush(atualPasso ? ativo : borderNeutro);
                numero.TextColor = atualPasso ? ativo : neutro;
                label.TextColor = atualPasso ? ativo : neutro;
                label.FontAttributes = atualPasso ? FontAttributes.Bold : FontAttributes.None;
                check.IsVisible = completo;
                numero.IsVisible = !completo;
            }

            Aplicar(view.Circulo1, view.Numero1, view.Label1, view.Check1, 1);
            Aplicar(view.Circulo2, view.Numero2, view.Label2, view.Check2, 2);
            Aplicar(view.Circulo3, view.Numero3, view.Label3, view.Check3, 3);
        }
    }
}
