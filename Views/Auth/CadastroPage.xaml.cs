using LOCATEM_DESKTOP.ViewModels.Auth;

namespace LOCATEM_DESKTOP.Views.Auth
{
    /// <summary>Migrado de pages/Auth/CadastroUsuario/CadastroUsuario.tsx.</summary>
    public partial class CadastroPage : ContentPage
    {
        public CadastroPage(CadastroViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
