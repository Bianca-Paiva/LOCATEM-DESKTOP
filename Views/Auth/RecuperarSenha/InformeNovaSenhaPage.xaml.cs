using LOCATEM_DESKTOP.ViewModels.Auth;

namespace LOCATEM_DESKTOP.Views.Auth.RecuperarSenha
{
    /// <summary>Migrado de pages/Auth/RecuperarSenha/InformeNovaSenha/InformeNovaSenha.tsx.</summary>
    public partial class InformeNovaSenhaPage : ContentPage
    {
        public InformeNovaSenhaPage(InformeNovaSenhaViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
