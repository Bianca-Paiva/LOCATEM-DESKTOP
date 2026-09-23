using LOCATEM_DESKTOP.Views.Auth;
using LOCATEM_DESKTOP.Views.Auth.RecuperarSenha;
using LOCATEM_DESKTOP.Views.Conta;
using LOCATEM_DESKTOP.Views.Home;

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
                RegisterInternalRoutes();
                _rotasRegistradas = true;
            }
        }

        // Rotas do fluxo de Login/Cadastro — não aparecem como itens de menu/flyout, só são
        // navegáveis via Shell.Current.GoToAsync("rota"), da mesma forma que o roteador por hash
        // do React (useRouter.ts) tratava essas telas.
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

        // Telas internas já migradas para o MAUI. Novas páginas devem entrar aqui conforme
        // forem implementadas, sem expô-las no Flyout do Shell.
        private static void RegisterInternalRoutes()
        {
            Routing.RegisterRoute("homeLocador", typeof(HomeLocadorPage));
            Routing.RegisterRoute("perfil", typeof(PerfilPage));
        }
    }
}
