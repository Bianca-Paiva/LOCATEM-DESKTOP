namespace LOCATEM_DESKTOP.Components.Home.HomeLocador
{
    /// <summary>
    /// Card de KPI do topo da Home do Locador.
    /// O ícone é definido diretamente na página HomeLocadorPage.
    /// </summary>
    public partial class HomeLocadorResumoCard : ContentView
    {
        public HomeLocadorResumoCard()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty LabelProperty =
            BindableProperty.Create(
                nameof(Label),
                typeof(string),
                typeof(HomeLocadorResumoCard),
                string.Empty);

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly BindableProperty ValorProperty =
            BindableProperty.Create(
                nameof(Valor),
                typeof(string),
                typeof(HomeLocadorResumoCard),
                string.Empty);

        public string Valor
        {
            get => (string)GetValue(ValorProperty);
            set => SetValue(ValorProperty, value);
        }

        public static readonly BindableProperty LegendaProperty =
            BindableProperty.Create(
                nameof(Legenda),
                typeof(string),
                typeof(HomeLocadorResumoCard),
                string.Empty,
                propertyChanged: OnTextosChanged);

        public string Legenda
        {
            get => (string)GetValue(LegendaProperty);
            set => SetValue(LegendaProperty, value);
        }

        public static readonly BindableProperty TendenciaProperty =
            BindableProperty.Create(
                nameof(Tendencia),
                typeof(string),
                typeof(HomeLocadorResumoCard),
                string.Empty,
                propertyChanged: OnTextosChanged);

        public string Tendencia
        {
            get => (string)GetValue(TendenciaProperty);
            set => SetValue(TendenciaProperty, value);
        }

        public static readonly BindableProperty HasTendenciaProperty =
            BindableProperty.Create(
                nameof(HasTendencia),
                typeof(bool),
                typeof(HomeLocadorResumoCard),
                false);

        public bool HasTendencia
        {
            get => (bool)GetValue(HasTendenciaProperty);
            private set => SetValue(HasTendenciaProperty, value);
        }

        public static readonly BindableProperty HasLegendaProperty =
            BindableProperty.Create(
                nameof(HasLegenda),
                typeof(bool),
                typeof(HomeLocadorResumoCard),
                false);

        public bool HasLegenda
        {
            get => (bool)GetValue(HasLegendaProperty);
            private set => SetValue(HasLegendaProperty, value);
        }

        public static readonly BindableProperty MostrarEstrelasProperty =
            BindableProperty.Create(
                nameof(MostrarEstrelas),
                typeof(bool),
                typeof(HomeLocadorResumoCard),
                false);

        public bool MostrarEstrelas
        {
            get => (bool)GetValue(MostrarEstrelasProperty);
            set => SetValue(MostrarEstrelasProperty, value);
        }

        public static readonly BindableProperty NotaProperty =
            BindableProperty.Create(
                nameof(Nota),
                typeof(double),
                typeof(HomeLocadorResumoCard),
                0d);

        public double Nota
        {
            get => (double)GetValue(NotaProperty);
            set => SetValue(NotaProperty, value);
        }

        public static readonly BindableProperty LegendaEstrelasProperty =
            BindableProperty.Create(
                nameof(LegendaEstrelas),
                typeof(string),
                typeof(HomeLocadorResumoCard),
                string.Empty);

        public string LegendaEstrelas
        {
            get => (string)GetValue(LegendaEstrelasProperty);
            set => SetValue(LegendaEstrelasProperty, value);
        }

        private static void OnTextosChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            var view = (HomeLocadorResumoCard)bindable;

            view.HasTendencia = !string.IsNullOrEmpty(view.Tendencia);

            view.HasLegenda =
                !view.HasTendencia &&
                !string.IsNullOrEmpty(view.Legenda);
        }
    }
}