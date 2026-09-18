using System.Collections.ObjectModel;
using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Auth;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Validation.Auth;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Auth
{
    /// <summary>
    /// Migrado de hooks/Auth/useCadastroForm.ts + pages/Auth/CadastroUsuario/CadastroUsuario.tsx.
    /// Concentra estado do formulário, máscaras, validação (via CadastroValidator) e o submit —
    /// a View (CadastroPage) fica só com bindings.
    /// </summary>
    public class CadastroViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        public CadastroViewModel(IAuthService authService)
        {
            _authService = authService;

            SubmitCommand = new AsyncRelayCommand(SubmitAsync, () => IsNotBusy);
            IrParaLoginCommand = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(".."));
            AbrirBuscaCepCommand = new AsyncRelayCommand(async () =>
                await Launcher.Default.OpenAsync("https://buscacepinter.correios.com.br/app/endereco/index.php"));
            FecharAlertaCommand = new RelayCommand(() => Alerta = null);
            ConfirmarSucessoCommand = new AsyncRelayCommand(async () =>
            {
                SuccessModalOpen = false;
                await Shell.Current.GoToAsync("..");
            });

            AtualizarValidacaoSenha();
        }

        // ===================== CAMPOS =====================

        private string _nome = string.Empty;
        public string Nome
        {
            get => _nome;
            set { if (SetProperty(ref _nome, value)) NomeError.Clear(); }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { if (SetProperty(ref _email, value)) EmailError.Clear(); }
        }

        private string _telefone = string.Empty;
        public string Telefone
        {
            get => _telefone;
            set
            {
                var mascarado = MaskHelper.MaskPhone(value);
                if (SetProperty(ref _telefone, mascarado)) TelefoneError.Clear();
            }
        }

        private string _documento = string.Empty;
        public string Documento
        {
            get => _documento;
            set
            {
                var mascarado = MaskHelper.MaskCnpj(value);
                if (SetProperty(ref _documento, mascarado)) DocumentoError.Clear();
            }
        }

        private string _cep = string.Empty;
        public string Cep
        {
            get => _cep;
            set
            {
                var mascarado = MaskHelper.MaskCep(value);
                if (SetProperty(ref _cep, mascarado)) CepError.Clear();
            }
        }

        private string _logradouro = string.Empty;
        public string Logradouro
        {
            get => _logradouro;
            set { if (SetProperty(ref _logradouro, value)) LogradouroError.Clear(); }
        }

        private string _numero = string.Empty;
        public string Numero
        {
            get => _numero;
            set { if (SetProperty(ref _numero, value)) NumeroError.Clear(); }
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
                    AtualizarValidacaoSenha();
                    OnPropertyChanged(nameof(ConfirmarSenhaStatus));
                }
            }
        }

        private string _confirmarSenha = string.Empty;
        public string ConfirmarSenha
        {
            get => _confirmarSenha;
            set
            {
                if (SetProperty(ref _confirmarSenha, value))
                {
                    ConfirmarSenhaError.Clear();
                    OnPropertyChanged(nameof(ConfirmarSenhaStatus));
                }
            }
        }

        // ===================== ESTADO DE ERRO / SHAKE POR CAMPO =====================

        public FieldErrorState NomeError { get; } = new();
        public FieldErrorState EmailError { get; } = new();
        public FieldErrorState TelefoneError { get; } = new();
        public FieldErrorState DocumentoError { get; } = new();
        public FieldErrorState CepError { get; } = new();
        public FieldErrorState LogradouroError { get; } = new();
        public FieldErrorState NumeroError { get; } = new();
        public FieldErrorState SenhaError { get; } = new();
        public FieldErrorState ConfirmarSenhaError { get; } = new();

        /// <summary>Mensagens de erro por campo — atualizadas a cada tentativa de submit (equivalente a "errors" do react-hook-form).</summary>
        private CadastroFieldErrors _campoErros = new();

        public string NomeErrorText => _campoErros.Nome ?? string.Empty;
        public string EmailErrorText => _campoErros.Email ?? string.Empty;
        public string TelefoneErrorText => _campoErros.Telefone ?? string.Empty;
        public string DocumentoErrorText => _campoErros.Documento ?? string.Empty;
        public string CepErrorText => _campoErros.Cep ?? string.Empty;
        public string LogradouroErrorText => _campoErros.Logradouro ?? string.Empty;
        public string NumeroErrorText => _campoErros.Numero ?? string.Empty;
        public string SenhaErrorText => _campoErros.Senha ?? string.Empty;
        public string ConfirmarSenhaErrorText => _campoErros.ConfirmarSenha ?? string.Empty;

        /// <summary>Status "sucesso" (borda verde) — mostrado quando não há erro e o confirmarSenha já foi digitado.</summary>
        public bool? ConfirmarSenhaStatus => ConfirmarSenhaError.Active
            ? false
            : PasswordValidator.GetConfirmPasswordStatus(Senha, ConfirmarSenha);

        // ===================== FORÇA DA SENHA =====================

        public bool MostrarMedidorSenha => Senha.Length > 0;

        private PasswordStrengthResult _strengthResult = PasswordValidator.CheckPasswordStrength(string.Empty);
        public PasswordStrengthResult StrengthResult
        {
            get => _strengthResult;
            private set => SetProperty(ref _strengthResult, value);
        }

        public ObservableCollection<PasswordValidationItem> PasswordValidationItems { get; } = new();

        private void AtualizarValidacaoSenha()
        {
            StrengthResult = PasswordValidator.CheckPasswordStrength(Senha);
            OnPropertyChanged(nameof(MostrarMedidorSenha));

            PasswordValidationItems.Clear();
            foreach (var item in PasswordValidator.GetPasswordValidations(Senha))
                PasswordValidationItems.Add(item);
        }

        // ===================== ALERTA / MODAL DE SUCESSO =====================

        private AlertaMessage? _alerta;
        public AlertaMessage? Alerta
        {
            get => _alerta;
            set
            {
                if (SetProperty(ref _alerta, value))
                    OnPropertyChanged(nameof(TemAlerta));
            }
        }

        public bool TemAlerta => Alerta is not null;

        private bool _successModalOpen;
        public bool SuccessModalOpen
        {
            get => _successModalOpen;
            set => SetProperty(ref _successModalOpen, value);
        }

        // ===================== COMMANDS =====================

        public ICommand SubmitCommand { get; }
        public ICommand IrParaLoginCommand { get; }
        public ICommand AbrirBuscaCepCommand { get; }
        public ICommand FecharAlertaCommand { get; }
        public ICommand ConfirmarSucessoCommand { get; }

        // ===================== SUBMIT =====================

        private async Task SubmitAsync()
        {
            var snapshot = new CadastroFormSnapshot
            {
                // O desktop é exclusivo para locadores — o tipo de conta não é mais escolhido no formulário.
                Tipo = TipoUsuario.Locador,
                Nome = Nome,
                Email = Email,
                Telefone = Telefone,
                Documento = Documento,
                Cep = Cep,
                Logradouro = Logradouro,
                Numero = Numero,
                Senha = Senha,
                ConfirmarSenha = ConfirmarSenha
            };

            _campoErros = CadastroValidator.Validate(snapshot);
            NotificarErrosDeCampo();

            if (!TratarSubmitInvalido(snapshot))
                await SubmitValidoAsync(snapshot);
        }

        /// <summary>Equivalente a onInvalidSubmit do useCadastroForm.ts. Retorna true se havia erro (bloqueando o submit).</summary>
        private bool TratarSubmitInvalido(CadastroFormSnapshot data)
        {
            var camposVazios = false;

            void Checar(string valor, FieldErrorState estado, string? erro)
            {
                if (string.IsNullOrWhiteSpace(valor))
                {
                    estado.Trigger();
                    camposVazios = true;
                }
                else if (erro is not null)
                {
                    estado.Trigger();
                }
            }

            Checar(data.Nome, NomeError, _campoErros.Nome);
            Checar(data.Email, EmailError, _campoErros.Email);
            Checar(data.Telefone, TelefoneError, _campoErros.Telefone);
            Checar(data.Documento, DocumentoError, _campoErros.Documento);
            Checar(data.Cep, CepError, _campoErros.Cep);
            Checar(data.Logradouro, LogradouroError, _campoErros.Logradouro);
            Checar(data.Numero, NumeroError, _campoErros.Numero);
            Checar(data.Senha, SenhaError, _campoErros.Senha);
            Checar(data.ConfirmarSenha, ConfirmarSenhaError, _campoErros.ConfirmarSenha);

            if (camposVazios)
            {
                Alerta = CadastroMessages.Required;
                return true;
            }

            if (_campoErros.Email is not null) { Alerta = CadastroMessages.InvalidEmail; return true; }
            if (_campoErros.Nome is not null) { Alerta = CadastroMessages.InvalidName; return true; }
            if (_campoErros.Telefone is not null) { Alerta = CadastroMessages.InvalidPhone; return true; }
            if (_campoErros.Documento is not null) { Alerta = CadastroMessages.InvalidCnpj; return true; }
            if (_campoErros.Cep is not null) { Alerta = CadastroMessages.InvalidCep; return true; }

            if (_campoErros.ConfirmarSenha == "As senhas não coincidem")
            {
                Alerta = PasswordMessages.Mismatch;
                return true;
            }

            if (_campoErros.Senha is not null)
            {
                Alerta = StrengthResult.Strength == PasswordStrength.Media ? PasswordMessages.Medium : PasswordMessages.Weak;
                return true;
            }

            return _campoErros.HasErrors;
        }

        private async Task SubmitValidoAsync(CadastroFormSnapshot data)
        {
            IsBusy = true;
            try
            {
                await _authService.CriarUsuarioAsync(new CadastroPayload
                {
                    Nome = data.Nome,
                    Email = data.Email,
                    Senha = data.Senha,
                    ConfirmarSenha = data.ConfirmarSenha,
                    Telefone = data.Telefone,
                    Documento = System.Text.RegularExpressions.Regex.Replace(data.Documento, @"\D", string.Empty),
                    TipoUsuario = 2 // Locador — este desktop é exclusivo para locadores.
                });

                SuccessModalOpen = true;
            }
            catch
            {
                Alerta = CadastroMessages.ApiError;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void NotificarErrosDeCampo()
        {
            OnPropertyChanged(nameof(NomeErrorText));
            OnPropertyChanged(nameof(EmailErrorText));
            OnPropertyChanged(nameof(TelefoneErrorText));
            OnPropertyChanged(nameof(DocumentoErrorText));
            OnPropertyChanged(nameof(CepErrorText));
            OnPropertyChanged(nameof(LogradouroErrorText));
            OnPropertyChanged(nameof(NumeroErrorText));
            OnPropertyChanged(nameof(SenhaErrorText));
            OnPropertyChanged(nameof(ConfirmarSenhaErrorText));
        }
    }
}