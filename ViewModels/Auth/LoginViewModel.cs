using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Auth;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Validation.Auth;
using LOCATEM_DESKTOP.ViewModels.Base;


namespace LOCATEM_DESKTOP.ViewModels.Auth
{
    /// <summary>Migrado de pages/Auth/Login/Login.tsx.</summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthSessionService _authSession;
        private readonly IRedirectAposLoginService _redirectService;

        public LoginViewModel(IAuthSessionService authSession, IRedirectAposLoginService redirectService)
        {
            _authSession = authSession;
            _redirectService = redirectService;

            EntrarCommand = new AsyncRelayCommand(EntrarAsync, () => IsNotBusy);
            EsqueceuSenhaCommand = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync("informeEmail"));
            CriarContaCommand = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync("cadastro"));
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    EmailError.Clear();
                    ErrorMessage = string.Empty;
                }
            }
        }

        private string _senha = string.Empty;
        public string Senha
        {
            get => _senha;
            set
            {
                if (SetProperty(ref _senha, value))
                {
                    SenhaError.Clear();
                    ErrorMessage = string.Empty;
                }
            }
        }

        public FieldErrorState EmailError { get; } = new();
        public FieldErrorState SenhaError { get; } = new();

        private string _errorMessage = string.Empty;
        /// <summary>Mensagem de erro geral ("E-mail ou senha inválidos.") — equivalente ao banner de erro do Login.</summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                    OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public string EmailErrorText => EmailError.Active && string.IsNullOrWhiteSpace(Email)
            ? "O e-mail é obrigatório"
            : EmailError.Active ? "Digite um e-mail válido" : string.Empty;

        public string SenhaErrorText => SenhaError.Active && string.IsNullOrWhiteSpace(Senha)
            ? "A senha é obrigatória"
            : string.Empty;

        public System.Windows.Input.ICommand EntrarCommand { get; }
        public System.Windows.Input.ICommand EsqueceuSenhaCommand { get; }
        public System.Windows.Input.ICommand CriarContaCommand { get; }

        private async Task EntrarAsync()
        {
            var possuiErro = false;

            if (string.IsNullOrWhiteSpace(Email) || !EmailValidator.IsValid(Email))
            {
                EmailError.Trigger();
                OnPropertyChanged(nameof(EmailErrorText));
                possuiErro = true;
            }

            if (string.IsNullOrWhiteSpace(Senha))
            {
                SenhaError.Trigger();
                OnPropertyChanged(nameof(SenhaErrorText));
                possuiErro = true;
            }

            if (possuiErro) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                // O AuthService (chamada HTTP real) ainda não está integrado a um backend real
                // neste fluxo — igual ao React, onde a chamada a loginUsuario() fica comentada e
                // o usuário autenticado é resolvido a partir do catálogo mockado.
                if (Senha == "erro-login")
                    throw new InvalidOperationException("Falha de autenticacao simulada");

                var usuario = _authSession.Login(Email);

                var rotaRedirect = _redirectService.LerRedirect();
                if (rotaRedirect is not null)
                {
                    _redirectService.LimparRedirect();
                    await Shell.Current.GoToAsync($"//{rotaRedirect}");
                }
                else if (usuario.Tipo == TipoUsuario.Locador)
                {
                    // O locador tem dashboard próprio e nunca cai no marketplace do locatário —
                    // mesma regra do Header/Home do React.
                    await Shell.Current.GoToAsync("homeLocador");
                }
                else
                {
                    // NOTA DE ESCOPO: a Home do locatário ainda não foi migrada para o MAUI.
                    // Assim que existir, troque a rota abaixo.
                    await Shell.Current.GoToAsync("//login");
                }
            }
            catch
            {
                ErrorMessage = "E-mail ou senha inválidos.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
