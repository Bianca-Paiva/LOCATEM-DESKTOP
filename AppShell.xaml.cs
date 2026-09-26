using LOCATEM_DESKTOP.Views.Auth;
using LOCATEM_DESKTOP.Views.Auth.RecuperarSenha;
using LOCATEM_DESKTOP.Views.Conta;
using LOCATEM_DESKTOP.Views.Ferramentas;
using LOCATEM_DESKTOP.Views.Home;
using LOCATEM_DESKTOP.Views.Locacoes;

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

        // Telas internas já migradas para o MAUI. Novas páginas devem entrar aqui conforme
        // forem implementadas, sem expô-las no Flyout do Shell.
        private static void RegisterInternalRoutes()
        {
            Routing.RegisterRoute("perfil", typeof(PerfilPage));
            Routing.RegisterRoute("minhasFerramentas", typeof(MinhasFerramentasPage));
            Routing.RegisterRoute("cadastroFerramenta", typeof(CadastroFerramentaPage));
            Routing.RegisterRoute("ferramentaDetalhe", typeof(FerramentaDetalhePage));
            Routing.RegisterRoute("gerenciarLocacoes", typeof(GerenciarLocacoesPage));
        }
    }
}
