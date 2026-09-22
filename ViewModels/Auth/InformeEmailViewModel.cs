using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Auth;
using LOCATEM_DESKTOP.Validation.Auth;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Auth
{
    /// <summary>Migrado de pages/Auth/RecuperarSenha/InformeEmail/InformeEmail.tsx.</summary>
    public class InformeEmailViewModel : BaseViewModel
    {
        public InformeEmailViewModel()
        {
            EnviarEmailCommand = new RelayCommand(EnviarEmail);
            IrParaLoginCommand = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync("//login"));
            IrParaCadastroCommand = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync("cadastro"));
            ContatoSuporteCommand = new AsyncRelayCommand(async () =>
                await Shell.Current.DisplayAlert("Suporte", "Esta funcionalidade está em desenvolvimento.", "OK"));
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                    EmailError.Clear();
            }
        }

        public FieldErrorState EmailError { get; } = new();

        public string EmailErrorText => EmailError.Active ? "Preencha de forma correta para continuar" : string.Empty;

        public System.Windows.Input.ICommand EnviarEmailCommand { get; }
        public System.Windows.Input.ICommand IrParaLoginCommand { get; }
        public System.Windows.Input.ICommand IrParaCadastroCommand { get; }
        public System.Windows.Input.ICommand ContatoSuporteCommand { get; }

        private async void EnviarEmail()
        {
            if (!EmailValidator.IsValid(Email))
            {
                EmailError.Trigger();
                OnPropertyChanged(nameof(EmailErrorText));
                return;
            }

            EmailError.Reset();

            // Simulação de envio de e-mail (mesmo comportamento do React): gera um código de 5
            // dígitos e guarda para a etapa de verificação conferir.
            var tokenGerado = new Random().Next(10000, 99999).ToString();
            Preferences.Default.Set("codigo_recuperacao", tokenGerado);

            System.Diagnostics.Debug.WriteLine("=====================================");
            System.Diagnostics.Debug.WriteLine("E-MAIL ENVIADO COM SUCESSO!");
            System.Diagnostics.Debug.WriteLine($"Destinatário: {Email}");
            System.Diagnostics.Debug.WriteLine($"Código de verificação: {tokenGerado}");
            System.Diagnostics.Debug.WriteLine("=====================================");

            await Shell.Current.GoToAsync("informeToken");
        }
    }
}
