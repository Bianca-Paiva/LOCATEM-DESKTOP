using System.Collections.ObjectModel;
using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Auth;
using LOCATEM_DESKTOP.Validation.Auth;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Auth
{
    /// <summary>Migrado de pages/Auth/RecuperarSenha/InformeNovaSenha/InformeNovaSenha.tsx.</summary>
    public class InformeNovaSenhaViewModel : BaseViewModel
    {
        public InformeNovaSenhaViewModel()
        {
            AlterarSenhaCommand = new RelayCommand(Submit);
            FecharAlertaCommand = new RelayCommand(() => Alerta = null);
            ConfirmarSucessoCommand = new AsyncRelayCommand(async () =>
            {
                SuccessModalOpen = false;
                await Shell.Current.GoToAsync("//login");
            });

            AtualizarValidacaoSenha();
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
                    OnPropertyChanged(nameof(ConfirmarSenhaErrorText));
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
                    OnPropertyChanged(nameof(ConfirmarSenhaErrorText));
                }
            }
        }

        public FieldErrorState SenhaError { get; } = new();
        public FieldErrorState ConfirmarSenhaError { get; } = new();

        public bool? ConfirmarSenhaStatus => ConfirmarSenhaError.Active
            ? false
            : PasswordValidator.GetConfirmPasswordStatus(Senha, ConfirmarSenha);

        public string ConfirmarSenhaErrorText => ConfirmarSenhaError.Active
            ? string.Empty
            : PasswordValidator.GetConfirmPasswordError(Senha, ConfirmarSenha);

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

        public ICommand AlterarSenhaCommand { get; }
        public ICommand FecharAlertaCommand { get; }
        public ICommand ConfirmarSucessoCommand { get; }

        private void Submit()
        {
            SenhaError.Reset();
            ConfirmarSenhaError.Reset();

            var resultado = PasswordValidator.ValidatePasswordForm(Senha, ConfirmarSenha, StrengthResult);

            switch (resultado)
            {
                case PasswordFormResultType.Required:
                    if (string.IsNullOrEmpty(Senha)) SenhaError.Trigger();
                    if (string.IsNullOrEmpty(ConfirmarSenha)) ConfirmarSenhaError.Trigger();
                    Alerta = PasswordMessages.Required;
                    break;

                case PasswordFormResultType.Mismatch:
                    ConfirmarSenhaError.Trigger();
                    Alerta = PasswordMessages.Mismatch;
                    break;

                case PasswordFormResultType.Fraca:
                    SenhaError.Trigger();
                    Alerta = PasswordMessages.Weak;
                    break;

                case PasswordFormResultType.Media:
                    SenhaError.Trigger();
                    Alerta = PasswordMessages.Medium;
                    break;

                case PasswordFormResultType.Success:
                    SuccessModalOpen = true;
                    break;
            }
        }
    }
}
