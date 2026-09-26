namespace LOCATEM_DESKTOP.Components.Shared
{
    /// <summary>
    /// Estado vazio de listas (ícone + título + descrição) — migrado de
    /// components/Locacoes/MinhasLocacoes/EstadoVazio/EstadoVazio.tsx.
    /// </summary>
    public partial class EstadoVazio : ContentView
    {
        public EstadoVazio()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TituloProperty =
            BindableProperty.Create(
                nameof(Titulo),
                typeof(string),
                typeof(EstadoVazio),
                string.Empty);

        public string Titulo
        {
            get => (string)GetValue(TituloProperty);
            set => SetValue(TituloProperty, value);
        }

        public static readonly BindableProperty DescricaoProperty =
            BindableProperty.Create(
                nameof(Descricao),
                typeof(string),
                typeof(EstadoVazio),
                string.Empty);

        public string Descricao
        {
            get => (string)GetValue(DescricaoProperty);
            set => SetValue(DescricaoProperty, value);
        }
    }
}
