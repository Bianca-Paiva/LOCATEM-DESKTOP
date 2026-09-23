namespace LOCATEM_DESKTOP.Components.Avaliacoes
{
    //
    // Fileira de 5 estrelas somente leitura — migrado de components/Avaliacoes/EstrelaAvaliacao.
    // A variante interativa (hover/clique) do React só é usada nas telas de Avaliação, ainda não migradas.
    //
    public partial class EstrelasAvaliacao : ContentView
    {
        private const int QuantidadeEstrelas = 5;

        private static readonly Color CorAtiva = Color.FromArgb("#F9C01A");
        private static readonly Color CorInativa = Color.FromArgb("#D9D9D9");

        private readonly List<Label> _estrelas = new();

        public EstrelasAvaliacao()
        {
            InitializeComponent();

            for (var i = 0; i < QuantidadeEstrelas; i++)
            {
                var estrela = new Label { Text = "★", FontSize = 14, TextColor = CorInativa, VerticalOptions = LayoutOptions.Center };
                _estrelas.Add(estrela);
                Fileira.Add(estrela);
            }

            Aplicar(Nota);
        }

        public static readonly BindableProperty NotaProperty =
            BindableProperty.Create(nameof(Nota), typeof(double), typeof(EstrelasAvaliacao), 0d, propertyChanged: OnNotaChanged);
        public double Nota { get => (double)GetValue(NotaProperty); set => SetValue(NotaProperty, value); }

        public static readonly BindableProperty TamanhoEstrelaProperty =
            BindableProperty.Create(nameof(TamanhoEstrela), typeof(double), typeof(EstrelasAvaliacao), 14d, propertyChanged: OnTamanhoChanged);
        public double TamanhoEstrela { get => (double)GetValue(TamanhoEstrelaProperty); set => SetValue(TamanhoEstrelaProperty, value); }

        private static void OnNotaChanged(BindableObject bindable, object oldValue, object newValue) =>
            ((EstrelasAvaliacao)bindable).Aplicar((double)newValue);

        private static void OnTamanhoChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (EstrelasAvaliacao)bindable;
            foreach (var estrela in view._estrelas) estrela.FontSize = (double)newValue;
        }

        private void Aplicar(double nota)
        {
            for (var i = 0; i < _estrelas.Count; i++)
                _estrelas[i].TextColor = i < nota ? CorAtiva : CorInativa;
        }
    }
}
