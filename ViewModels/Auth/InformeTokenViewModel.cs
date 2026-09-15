using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Auth
{
    /// <summary>Migrado de pages/Auth/RecuperarSenha/InformeToken/InformeToken.tsx.</summary>
    public class InformeTokenViewModel : BaseViewModel, IDisposable
    {
        private const int DuracaoReenvioSegundos = 50;
        private IDispatcherTimer? _timer;

        public InformeTokenViewModel()
        {
            VerificarCommand = new RelayCommand(VerificarToken);
            ReenviarCommand = new RelayCommand(ReenviarCodigo, () => TimeLeft == 0);

            IniciarContagem();
        }

        private string _token = string.Empty;
        public string Token
        {
            get => _token;
            set
            {
                if (SetProperty(ref _token, value))
                {
                    if (HasError) HasError = false;
                    if (!string.IsNullOrEmpty(ErrorMessage)) ErrorMessage = string.Empty;
                }
            }
        }

        private int _timeLeft = DuracaoReenvioSegundos;
        public int TimeLeft
        {
            get => _timeLeft;
            private set
            {
                if (SetProperty(ref _timeLeft, value))
                {
                    OnPropertyChanged(nameof(ReenviarTexto));
                    (ReenviarCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string ReenviarTexto => TimeLeft > 0 ? $"Reenviar código em {TimeLeft}s" : "Reenviar código";

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                    OnPropertyChanged(nameof(TemErro));
            }
        }

        public bool TemErro => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand VerificarCommand { get; }
        public ICommand ReenviarCommand { get; }

        private void IniciarContagem()
        {
            TimeLeft = DuracaoReenvioSegundos;
            _timer = Application.Current?.Dispatcher.CreateTimer();
            if (_timer is null) return;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (_, _) =>
            {
                if (TimeLeft <= 0)
                {
                    _timer?.Stop();
                    return;
                }
                TimeLeft--;
            };
            _timer.Start();
        }

        private async void VerificarToken()
        {
            var tokenCorreto = Preferences.Default.Get<string?>("codigo_recuperacao", null);

            if (Token.Length < 5)
            {
                ErrorMessage = "Preencha todos os 5 dígitos do código.";
                HasError = false;
                await Task.Delay(10);
                HasError = true;
                return;
            }

            if (Token != tokenCorreto)
            {
                ErrorMessage = "Código incorreto. Verifique e tente novamente.";
                HasError = false;
                await Task.Delay(10);
                HasError = true;
                return;
            }

            HasError = false;
            ErrorMessage = string.Empty;
            Preferences.Default.Remove("codigo_recuperacao");

            await Shell.Current.GoToAsync("informeNovaSenha");
        }

        private void ReenviarCodigo()
        {
            var novoToken = new Random().Next(10000, 99999).ToString();
            Preferences.Default.Set("codigo_recuperacao", novoToken);

            System.Diagnostics.Debug.WriteLine("=====================================");
            System.Diagnostics.Debug.WriteLine("NOVO E-MAIL ENVIADO COM SUCESSO!");
            System.Diagnostics.Debug.WriteLine($"Novo Código de verificação: {novoToken}");
            System.Diagnostics.Debug.WriteLine("=====================================");

            _timer?.Stop();
            Token = string.Empty;
            HasError = false;
            ErrorMessage = string.Empty;
            IniciarContagem();
        }

        public void Dispose()
        {
            _timer?.Stop();
        }
    }
}
