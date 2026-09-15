namespace LOCATEM_DESKTOP.Components.Auth.RecuperarSenha
{
    /// <summary>
    /// Migrado de components/Auth/RecuperarSenha/TokenInput/TokenInput.tsx — 5 caixinhas de
    /// dígito com avanço automático de foco. A lógica de foco/backspace é puramente de UI
    /// (equivalente ao inputsRef.current[...].focus() do React), então fica aqui no code-behind;
    /// a validação em si (token correto ou não) continua no InformeTokenViewModel.
    /// </summary>
    public partial class TokenInputView : ContentView
    {
        private readonly Entry[] _entries;
        private readonly Border[] _boxes;
        private bool _updatingFromValue;

        public TokenInputView()
        {
            InitializeComponent();
            _entries = new[] { Entry0, Entry1, Entry2, Entry3, Entry4 };
            _boxes = new[] { Box0, Box1, Box2, Box3, Box4 };
        }

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(string), typeof(TokenInputView), string.Empty, BindingMode.TwoWay, propertyChanged: OnValueChanged);
        public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public static readonly BindableProperty HasErrorProperty =
            BindableProperty.Create(nameof(HasError), typeof(bool), typeof(TokenInputView), false, propertyChanged: OnHasErrorChanged);
        public bool HasError { get => (bool)GetValue(HasErrorProperty); set => SetValue(HasErrorProperty, value); }

        private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (TokenInputView)bindable;
            if (view._updatingFromValue) return;

            var valor = (newValue as string) ?? string.Empty;
            for (var i = 0; i < view._entries.Length; i++)
            {
                var caractere = i < valor.Length ? valor[i].ToString() : string.Empty;
                if (view._entries[i].Text != caractere)
                    view._entries[i].Text = caractere;
            }
        }

        private static void OnHasErrorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (TokenInputView)bindable;
            var erro = (bool)newValue;
            var cor = erro ? (Color)Application.Current!.Resources["ErrorColor"] : Color.FromArgb("#D0D5DD");
            foreach (var box in view._boxes) box.Stroke = new SolidColorBrush(cor);
        }

        private void OnDigitTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry) return;
            var index = Array.IndexOf(_entries, entry);
            if (index < 0) return;

            // Mantém só o último caractere digitado, como no React (text.slice(-1)).
            if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length > 1)
            {
                entry.Text = e.NewTextValue[^1].ToString();
                return;
            }

            AtualizarValueDosEntries();

            if (!string.IsNullOrEmpty(e.NewTextValue) && index < _entries.Length - 1)
            {
                _entries[index + 1].Focus();
            }
            // NOTA: o React usa onKeyDown para detectar Backspace numa caixa já vazia e voltar o
            // foco. O Entry do MAUI não expõe esse evento de tecla de forma multiplataforma sem um
            // handler nativo por plataforma, então esse detalhe (puramente de UX de foco) não foi
            // replicado 1:1 aqui — o preenchimento/avanço automático de foco continua igual.
        }

        private void AtualizarValueDosEntries()
        {
            _updatingFromValue = true;
            Value = string.Concat(_entries.Select(en => en.Text ?? string.Empty));
            _updatingFromValue = false;
        }
    }
}
