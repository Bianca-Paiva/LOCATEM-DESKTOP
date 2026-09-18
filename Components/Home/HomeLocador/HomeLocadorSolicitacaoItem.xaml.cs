using System.Windows.Input;

namespace LOCATEM_DESKTOP.Components.Home.HomeLocador
{
    /// <summary>
    /// Linha da seção "Solicitações Recentes" — migrado de HomeLocadorSolicitacaoItem.tsx.
    /// O BindingContext é a própria Locacao (a Home mostra os mesmos dados de Gerenciar Locações).
    /// </summary>
    public partial class HomeLocadorSolicitacaoItem : ContentView
    {
        public HomeLocadorSolicitacaoItem()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty VerDetalhesCommandProperty =
            BindableProperty.Create(nameof(VerDetalhesCommand), typeof(ICommand), typeof(HomeLocadorSolicitacaoItem));
        public ICommand? VerDetalhesCommand { get => (ICommand?)GetValue(VerDetalhesCommandProperty); set => SetValue(VerDetalhesCommandProperty, value); }
    }
}
