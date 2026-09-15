using System.Windows.Input;

namespace LOCATEM_DESKTOP.Components.Auth
{
    /// <summary>
    /// Migrado de components/Auth/RecuperarSenha/Alerta/Alerta.tsx. Fecha sozinho depois de 15s,
    /// igual ao original, além de expor CloseCommand para o "X".
    /// </summary>
    public partial class AlertaBanner : ContentView
    {
        private CancellationTokenSource? _autoCloseCts;

        public AlertaBanner()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TituloProperty =
            BindableProperty.Create(nameof(Titulo), typeof(string), typeof(AlertaBanner), string.Empty, propertyChanged: OnConteudoChanged);
        public string Titulo { get => (string)GetValue(TituloProperty); set => SetValue(TituloProperty, value); }

        public static readonly BindableProperty MensagemProperty =
            BindableProperty.Create(nameof(Mensagem), typeof(string), typeof(AlertaBanner), string.Empty, propertyChanged: OnConteudoChanged);
        public string Mensagem { get => (string)GetValue(MensagemProperty); set => SetValue(MensagemProperty, value); }

        public static readonly BindableProperty TemMensagemProperty =
            BindableProperty.Create(nameof(TemMensagem), typeof(bool), typeof(AlertaBanner), false);
        public bool TemMensagem { get => (bool)GetValue(TemMensagemProperty); private set => SetValue(TemMensagemProperty, value); }

        public static readonly BindableProperty CloseCommandProperty =
            BindableProperty.Create(nameof(CloseCommand), typeof(ICommand), typeof(AlertaBanner));
        public ICommand? CloseCommand { get => (ICommand?)GetValue(CloseCommandProperty); set => SetValue(CloseCommandProperty, value); }

        private static void OnConteudoChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (AlertaBanner)bindable;
            view.TemMensagem = !string.IsNullOrEmpty(view.Mensagem);

            if (bindable.GetValue(TituloProperty) is string t && !string.IsNullOrEmpty(t))
                view.AgendarFechamentoAutomatico();
        }

        // Equivalente ao setTimeout(onClose, 15000) do useEffect original.
        private void AgendarFechamentoAutomatico()
        {
            _autoCloseCts?.Cancel();
            _autoCloseCts = new CancellationTokenSource();
            var token = _autoCloseCts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(15), token);
                    if (!token.IsCancellationRequested)
                        MainThread.BeginInvokeOnMainThread(() => CloseCommand?.Execute(null));
                }
                catch (TaskCanceledException) { }
            });
        }

        private void OnCloseTapped(object? sender, TappedEventArgs e)
        {
            _autoCloseCts?.Cancel();
            CloseCommand?.Execute(null);
        }
    }
}
