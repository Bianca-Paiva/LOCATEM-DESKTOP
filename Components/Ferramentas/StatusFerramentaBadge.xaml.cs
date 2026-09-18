using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Components.Ferramentas
{
    //
    // Migrado de components/Ferramentas/MinhasFerramentas/StatusFerramentaBadge — exibido sobre a
    // miniatura do card, na versão compacta usada pela Home.
    //
    public partial class StatusFerramentaBadge : ContentView
    {
        public StatusFerramentaBadge()
        {
            InitializeComponent();
            Aplicar(Status);
        }

        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create(nameof(Status), typeof(StatusFerramenta), typeof(StatusFerramentaBadge),
                StatusFerramenta.Disponivel, propertyChanged: OnStatusChanged);
        public StatusFerramenta Status { get => (StatusFerramenta)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

        private static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue) =>
            ((StatusFerramentaBadge)bindable).Aplicar((StatusFerramenta)newValue);

        private void Aplicar(StatusFerramenta status)
        {
            var config = StatusFerramentaConfig.Obter(status);
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
