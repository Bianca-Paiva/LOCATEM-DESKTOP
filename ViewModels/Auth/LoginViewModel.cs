using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Auth;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Validation.Auth;
using LOCATEM_DESKTOP.ViewModels.Base;
using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.ViewModels.Auth
{
    /// <summary>Migrado de pages/Auth/Login/Login.tsx.</summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthSessionService _authSession;
        private readonly IRedirectAposLoginService _redirectService;
        private readonly IAuthService _authService;

        public LoginViewModel(
        IAuthSessionService authSession,
        IRedirectAposLoginService redirectService,
        IAuthService authService)
        {
            _authSession = authSession;
            _redirectService = redirectService;
            _authService = authService;

            EntrarCommand = new AsyncRelayCommand(EntrarAsync, () => IsNotBusy);
            EsqueceuSenhaCommand = new AsyncRelayCommand(
                async () => await Shell.Current.GoToAsync("informeEmail"));
            CriarContaCommand = new AsyncRelayCommand(
                async () => await Shell.Current.GoToAsync("cadastro"));
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

        private string _successMessage = string.Empty;

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                if (SetProperty(ref _successMessage, value))
                    OnPropertyChanged(nameof(HasSuccess));
            }
        }

        public bool HasSuccess => !string.IsNullOrEmpty(SuccessMessage);

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
            SuccessMessage = string.Empty;

            try
            {
                var emailNormalizado = Email.Trim();
                Usuario usuario;

                // João é o único usuário mockado e autentica 100% localmente como locador.
                // Se o e-mail for da conta de teste, nunca tentamos acessar a API —
                // inclusive quando a senha estiver incorreta.
                if (UsuariosMock.EhUsuarioDeTeste(emailNormalizado))
                {
                    usuario = UsuariosMock.AutenticarUsuarioTeste(emailNormalizado, Senha)
                        ?? throw new InvalidOperationException("E-mail ou senha inválidos.");
                }
                else
                {
                    // Usuários reais continuam usando exatamente o fluxo existente da API.
                    var resultado = await _authService.LoginAsync(
                        new LoginPayload
                        {
                            Email = emailNormalizado,
                            Senha = Senha
                        }
                    );

                    var perfil = await _authService.BuscarUsuarioLogadoAsync(resultado.Token);

                    if (!Enum.TryParse<TipoUsuario>(
                        perfil.TipoUsuario,
                        true,
                        out var tipoUsuario))
                    {
                        throw new InvalidOperationException(
                            "Tipo de usuário retornado pela API é inválido."
                        );
                    }

                    usuario = new Usuario
                    {
                        Id = perfil.Id.ToString(),
                        Nome = perfil.Nome,
                        Email = perfil.Email,
                        Telefone = perfil.Telefone,
                        Documento = perfil.Documento,
                        Endereco = perfil.Endereco ?? string.Empty,
                        Tipo = tipoUsuario,
                        FotoUrl = perfil.FotoUrl,
                        EmailVerificado = false,
                        Desde = perfil.Desde,
                        Reputacao = new ReputacaoUsuario
                        {
                            Rating = perfil.Reputacao.Rating,
                            TotalAvaliacoes = perfil.Reputacao.TotalAvaliacoes,
                            LocacoesConcluidas = perfil.Reputacao.LocacoesConcluidas,
                            EntregasNoPrazoPercentual =
                                perfil.Reputacao.EntregasNoPrazoPercentual
                        },
                        Token = resultado.Token
                    };
                }

                _authSession.DefinirUsuario(usuario);

                SuccessMessage = "Login concluído com sucesso!";
                await Task.Delay(1500);

                // A Home do Locador é a primeira tela da área autenticada.
                if (usuario.Tipo == TipoUsuario.Locador)
                {
                    _redirectService.LimparRedirect();
                    await Shell.Current.GoToAsync("//homeLocador");
                }
                else
                {
                    // Mantém o comportamento anterior para outros perfis vindos da API.
                    var rotaRedirect = _redirectService.LerRedirect();

                    if (rotaRedirect is not null)
                    {
                        _redirectService.LimparRedirect();
                        await Shell.Current.GoToAsync($"//{rotaRedirect}");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//login");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
