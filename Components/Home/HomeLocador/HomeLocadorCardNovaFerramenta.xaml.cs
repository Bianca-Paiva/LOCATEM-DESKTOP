using System.Windows.Input;

namespace LOCATEM_DESKTOP.Components.Home.HomeLocador
{
    /// <summary>Card "+ Cadastrar nova ferramenta" ao final da grade — migrado de HomeLocadorCardNovaFerramenta.tsx.</summary>
    public partial class HomeLocadorCardNovaFerramenta : ContentView
    {
        public HomeLocadorCardNovaFerramenta()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CadastrarCommandProperty =
            BindableProperty.Create(nameof(CadastrarCommand), typeof(ICommand), typeof(HomeLocadorCardNovaFerramenta));
        public ICommand? CadastrarCommand { get => (ICommand?)GetValue(CadastrarCommandProperty); set => SetValue(CadastrarCommandProperty, value); }
    }
}
