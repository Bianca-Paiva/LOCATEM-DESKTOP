using LOCATEM_DESKTOP.ViewModels.Auth;

namespace LOCATEM_DESKTOP.Views.Auth.RecuperarSenha
{
    /// <summary>Migrado de pages/Auth/RecuperarSenha/InformeEmail/InformeEmail.tsx.</summary>
    public partial class InformeEmailPage : ContentPage
    {
        public InformeEmailPage(InformeEmailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
