using Microsoft.Extensions.Logging;
using MauiIcons.Material;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Ferramentas;
using LOCATEM_DESKTOP.Services.Locacoes;
using LOCATEM_DESKTOP.ViewModels.Auth;
using LOCATEM_DESKTOP.ViewModels.Home;
using LOCATEM_DESKTOP.Views.Auth;
using LOCATEM_DESKTOP.Views.Auth.RecuperarSenha;
using LOCATEM_DESKTOP.Views.Home;

namespace LOCATEM_DESKTOP
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMaterialMauiIcons() // AathifMahir.Maui.MauiIcons.Material — usado nos ícones do fluxo de Auth
                .ConfigureFonts(fonts =>
                {

                    // Fonte usada em todo o fluxo de Auth no React (styles/global.css: font-family 'Inter').
                    // Os arquivos já existiam em Resources/Fonts mas ainda não estavam registrados.
                    fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                    fonts.AddFont("Inter-Medium.ttf", "InterMedium");
                    fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                    fonts.AddFont("Inter-Bold.ttf", "InterBold");
                    fonts.AddFont("Inter-ExtraBold.ttf", "InterExtraBold");
                    fonts.AddFont("Inter-Italic.ttf", "InterItalic");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            RegisterServices(builder.Services);
            RegisterViewModels(builder.Services);
            RegisterPages(builder.Services);

            return builder.Build();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Sessão do usuário autenticado — singleton, equivalente ao AuthContext/AuthProvider do React.
            services.AddSingleton<IAuthSessionService, AuthSessionService>();

            // Chamadas HTTP de autenticação (Cadastro/Login) — migrado de services/authService.ts.
            services.AddSingleton<IAuthService, AuthService>();

            // Redirecionamento pós-login (utils/Auth/redirectAposLogin.ts).
            services.AddSingleton<IRedirectAposLoginService, RedirectAposLoginService>();
            // Catálogo de ferramentas e locações — singletons
            services.AddSingleton<ICatalogoService, CatalogoService>();
            services.AddSingleton<ILocacaoService, LocacaoService>();

        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<LoginViewModel>();
            services.AddTransient<CadastroViewModel>();
            services.AddTransient<InformeEmailViewModel>();
            services.AddTransient<InformeTokenViewModel>();
            services.AddTransient<InformeNovaSenhaViewModel>();
            services.AddTransient<HomeLocadorViewModel>();
        }

        private static void RegisterPages(IServiceCollection services)
        {
            services.AddTransient<LoginPage>();
            services.AddTransient<CadastroPage>();
            services.AddTransient<InformeEmailPage>();
            services.AddTransient<InformeTokenPage>();
            services.AddTransient<InformeNovaSenhaPage>();
            services.AddTransient<HomeLocadorPage>();
        }
    }
}
