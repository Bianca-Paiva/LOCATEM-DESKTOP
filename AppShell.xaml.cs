using LOCATEM_DESKTOP.Views.Auth;
using LOCATEM_DESKTOP.Views.Auth.RecuperarSenha;

namespace LOCATEM_DESKTOP
{
    public partial class AppShell : Shell
    {
        private static bool _rotasRegistradas;

        public AppShell()
        {
            InitializeComponent();

            if (!_rotasRegistradas)
            {
                RegisterAuthRoutes();
                _rotasRegistradas = true;
            }
        }

        // Rotas auxiliares do fluxo de autenticação. Login e HomeLocador são rotas raiz
        // declaradas no AppShell.xaml para permitir navegação absoluta com //rota.
        private static void RegisterAuthRoutes()
        {
            Routing.RegisterRoute(
                "cadastro",
                typeof(CadastroPage)
            );

            Routing.RegisterRoute(
                "informeEmail",
                typeof(InformeEmailPage)
            );

            Routing.RegisterRoute(
                "informeToken",
                typeof(InformeTokenPage)
            );

            Routing.RegisterRoute(
                "informeNovaSenha",
                typeof(InformeNovaSenhaPage)
            );
        }
    }
}
