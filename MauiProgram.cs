using Microsoft.Extensions.Logging;

using MauiIcons.Core;
using MauiIcons.Material;
using MauiIcons.Material.Outlined;

using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Ferramentas;
using LOCATEM_DESKTOP.Services.Locacoes;
using LOCATEM_DESKTOP.Services.Notificacoes;
using LOCATEM_DESKTOP.ViewModels.Avaliacoes;
using LOCATEM_DESKTOP.ViewModels.Auth;
using LOCATEM_DESKTOP.ViewModels.Conta;
using LOCATEM_DESKTOP.ViewModels.Conta.Notificacoes;
using LOCATEM_DESKTOP.ViewModels.Ferramentas;
using LOCATEM_DESKTOP.ViewModels.Home;
using LOCATEM_DESKTOP.ViewModels.Locacoes;
using LOCATEM_DESKTOP.Views.Avaliacoes;
using LOCATEM_DESKTOP.Views.Auth;
using LOCATEM_DESKTOP.Views.Auth.RecuperarSenha;
using LOCATEM_DESKTOP.Views.Conta;
using LOCATEM_DESKTOP.Views.Conta.Notificacoes;
using LOCATEM_DESKTOP.Views.Ferramentas;
using LOCATEM_DESKTOP.Views.Home;
using LOCATEM_DESKTOP.Views.Locacoes;
using Microsoft.Maui.Handlers;
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

                .UseMauiIconsCore(options =>
                {
                    options.SetDefaultIconSize(30.0);
                    options.SetDefaultIconAutoScaling(true);
                    options.SetDefaultFontOverride(true);
                })

                .UseMaterialMauiIcons()
                .UseMaterialOutlinedMauiIcons()

                .ConfigureFonts(fonts =>
                {
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

            // Remove a borda/linha azul padrão dos Entry no Windows.
#if WINDOWS
            EntryHandler.Mapper.AppendToMapping(
                "RemoveWindowsEntryBorder",
                (handler, view) =>
                {
                    var transparentBrush =
                        new Microsoft.UI.Xaml.Media.SolidColorBrush(
                            Microsoft.UI.Colors.Transparent
                        );

                    handler.PlatformView.BorderThickness =
                        new Microsoft.UI.Xaml.Thickness(0);

                    handler.PlatformView.BorderBrush =
                        transparentBrush;

                    handler.PlatformView.Background =
                        transparentBrush;

                    handler.PlatformView.Resources["TextControlBorderBrush"] =
                        transparentBrush;

                    handler.PlatformView.Resources["TextControlBorderBrushFocused"] =
                        transparentBrush;

                    handler.PlatformView.Resources["TextControlBorderBrushPointerOver"] =
                        transparentBrush;
                });

                // Mantém apenas a borda externa definida no XAML para os Editor no Windows.
                EditorHandler.Mapper.AppendToMapping(
                    "RemoveWindowsEditorBorder",
                    (handler, view) =>
                    {
                        var transparente = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                            Microsoft.UI.Colors.Transparent);

                        handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                        handler.PlatformView.BorderBrush = transparente;
                        handler.PlatformView.Background = transparente;

                        handler.PlatformView.Resources["TextControlBorderBrush"] = transparente;
                        handler.PlatformView.Resources["TextControlBorderBrushFocused"] = transparente;
                        handler.PlatformView.Resources["TextControlBorderBrushPointerOver"] = transparente;
                        handler.PlatformView.Resources["TextControlBorderThemeThickness"] =
                            new Microsoft.UI.Xaml.Thickness(0);
                        handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] =
                            new Microsoft.UI.Xaml.Thickness(0);
                });
                PickerHandler.Mapper.AppendToMapping("RemovePickerBorder", (handler, view) =>
{
                    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    handler.PlatformView.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.Colors.Transparent);

                    handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.Colors.Transparent);
                });
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

            // Catálogo de ferramentas e locações — singletons.
            services.AddSingleton<ICatalogoService, CatalogoService>();
            services.AddSingleton<ICadastroFerramentaService, CadastroFerramentaService>();
            services.AddSingleton<ILocacaoService, LocacaoService>();

            // Mantém o estado das notificações enquanto o aplicativo estiver aberto.
            services.AddSingleton<INotificacaoService, NotificacaoService>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<LoginViewModel>();
            services.AddTransient<CadastroViewModel>();
            services.AddTransient<InformeEmailViewModel>();
            services.AddTransient<InformeTokenViewModel>();
            services.AddTransient<InformeNovaSenhaViewModel>();
            services.AddTransient<HomeLocadorViewModel>();
            services.AddTransient<PerfilViewModel>();
            services.AddTransient<MinhasFerramentasViewModel>();
            services.AddTransient<CadastroFerramentaViewModel>();
            services.AddTransient<FerramentaDetalheViewModel>();
            services.AddTransient<GerenciarLocacoesViewModel>();
            services.AddTransient<HistoricoLocacoesViewModel>();
            // ViewModel da tela de Avaliações, derivada das locações finalizadas do locador.
            services.AddTransient<AvaliacaoViewModel>();
            // ViewModel da tela de Notificações, com filtro, paginação e modal de detalhes.
            services.AddTransient<NotificacoesViewModel>();
        }

        private static void RegisterPages(IServiceCollection services)
        {
            services.AddTransient<LoginPage>();
            services.AddTransient<CadastroPage>();
            services.AddTransient<InformeEmailPage>();
            services.AddTransient<InformeTokenPage>();
            services.AddTransient<InformeNovaSenhaPage>();
            services.AddTransient<HomeLocadorPage>();
            services.AddTransient<PerfilPage>();
            services.AddTransient<MinhasFerramentasPage>();
            services.AddTransient<CadastroFerramentaPage>();
            services.AddTransient<FerramentaDetalhePage>();
            services.AddTransient<GerenciarLocacoesPage>();
            services.AddTransient<HistoricoLocacoesPage>();
            // Página de Avaliações registrada no mesmo ciclo transient das demais telas internas.
            services.AddTransient<AvaliacaoPage>();
            // Página de Notificações registrada no mesmo ciclo transient das telas internas.
            services.AddTransient<NotificacoesPage>();
        }
    }
}
