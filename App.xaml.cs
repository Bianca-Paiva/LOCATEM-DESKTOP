using MauiIcons.Core;

namespace LOCATEM_DESKTOP
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Workaround documentado da lib (dotnet/maui#7503): o xmlns estilo URL usado pelo
            // mi: (AathifMahir.Maui.MauiIcons) pode falhar ao resolver no primeiro uso se o
            // assembly da lib ainda não foi tocado. Uma instância descartável força o load cedo.
            _ = new MauiIcon();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}