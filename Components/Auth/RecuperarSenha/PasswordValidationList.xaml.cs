using System.Collections;

namespace LOCATEM_DESKTOP.Components.Auth.RecuperarSenha
{
    /// <summary>Migrado de components/Auth/RecuperarSenha/PasswordValidationList/PasswordValidationList.tsx.</summary>
    public partial class PasswordValidationList : ContentView
    {
        public PasswordValidationList()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TituloProperty =
            BindableProperty.Create(nameof(Titulo), typeof(string), typeof(PasswordValidationList), "Dicas de segurança");
        public string Titulo { get => (string)GetValue(TituloProperty); set => SetValue(TituloProperty, value); }

        public static readonly BindableProperty ItensProperty =
            BindableProperty.Create(nameof(Itens), typeof(IEnumerable), typeof(PasswordValidationList));
        public IEnumerable? Itens { get => (IEnumerable?)GetValue(ItensProperty); set => SetValue(ItensProperty, value); }
    }
}
