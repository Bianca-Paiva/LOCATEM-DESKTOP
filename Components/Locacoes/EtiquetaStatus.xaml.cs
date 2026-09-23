using LOCATEM_DESKTOP.Models.Locacoes;

namespace LOCATEM_DESKTOP.Components.Locacoes
{
    //Migrado de components/Locacoes/MinhasLocacoes/EtiquetaStatus/EtiquetaStatus.tsx.
    public partial class EtiquetaStatus : ContentView
    {
        public EtiquetaStatus()
        {
            InitializeComponent();
            Aplicar(Status);
        }

        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create(nameof(Status), typeof(StatusLocacao), typeof(EtiquetaStatus),
                StatusLocacao.Pendente, propertyChanged: OnStatusChanged);
        public StatusLocacao Status { get => (StatusLocacao)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

        private static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue) =>
            ((EtiquetaStatus)bindable).Aplicar((StatusLocacao)newValue);

        private void Aplicar(StatusLocacao status)
        {
            var config = StatusLocacaoConfig.Obter(status);
            var cor = Color.FromArgb(config.Cor);

            Texto.Text = config.Label;
            Texto.TextColor = cor;
            Icone.Icon = config.Icone;
            Icone.IconColor = cor;
            Pilula.BackgroundColor = Color.FromArgb(config.Fundo);
            Pilula.Stroke = new SolidColorBrush(Color.FromArgb(config.Borda));
        }
    }
}
