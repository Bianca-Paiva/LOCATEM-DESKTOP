using LOCATEM_DESKTOP.ViewModels.Home;

namespace LOCATEM_DESKTOP.Views.Home
{
    /// <summary>Migrado de pages/Home/HomeLocador/HomeLocador.tsx.</summary>
    public partial class HomeLocadorPage : ContentPage
    {
        private readonly HomeLocadorViewModel _viewModel;

        public HomeLocadorPage(HomeLocadorViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Guarda de perfil + recarga dos dados a cada exibição, como os useMemo do hook React
            // recalculam quando o usuário ou as listas mudam.
            if (await _viewModel.GarantirAcessoAsync())
                _viewModel.Carregar();
        }
    }
}
