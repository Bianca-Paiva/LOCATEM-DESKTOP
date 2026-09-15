using LOCATEM_DESKTOP.ViewModels.Auth;

namespace LOCATEM_DESKTOP.Views.Auth
{
    /// <summary>Migrado de pages/Auth/Login/Login.tsx. Code-behind mínimo: só injeta o ViewModel.</summary>
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
