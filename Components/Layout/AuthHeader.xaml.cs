using System.Windows.Input;

namespace LOCATEM_DESKTOP.Components.Layout
{
    /// <summary>
    /// Migrado de components/Layout/Header/AuthHeader/AuthHeader.tsx — cabeçalho com gradiente e
    /// logo, usado no topo de todas as telas de Login/Cadastro/RecuperarSenha.
    /// Reaproveita a logo já existente em Resources/Images/LogoIcon.png.
    /// </summary>
    public partial class AuthHeader : ContentView
    {
        public AuthHeader()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Comando disparado ao tocar na logo (equivalente a navigate('home')). Se não for
        /// informado, não faz nada — a Home ainda não foi migrada para o MAUI (fora do escopo
        /// desta tarefa de Login/Cadastro).
        /// </summary>
        public static readonly BindableProperty LogoTappedCommandProperty =
            BindableProperty.Create(nameof(LogoTappedCommand), typeof(ICommand), typeof(AuthHeader));
        public ICommand? LogoTappedCommand { get => (ICommand?)GetValue(LogoTappedCommandProperty); set => SetValue(LogoTappedCommandProperty, value); }

        private void OnLogoTapped(object? sender, TappedEventArgs e) => LogoTappedCommand?.Execute(null);
    }
}
