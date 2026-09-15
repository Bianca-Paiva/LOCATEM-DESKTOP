using LOCATEM_DESKTOP.ViewModels.Auth;

namespace LOCATEM_DESKTOP.Views.Auth.RecuperarSenha
{
    /// <summary>Migrado de pages/Auth/RecuperarSenha/InformeToken/InformeToken.tsx.</summary>
    public partial class InformeTokenPage : ContentPage
    {
        private readonly InformeTokenViewModel _viewModel;

        public InformeTokenPage(InformeTokenViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);
            _viewModel.Dispose();
        }
    }
}
