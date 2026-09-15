using System.Windows.Input;
using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Components.Auth
{
    /// <summary>
    /// Migrado de components/Auth/CardOpcaoConta/CardOpcaoConta.tsx — card selecionável de tipo de
    /// conta (Locatário/Locador) usado no Cadastro.
    /// </summary>
    public partial class CardOpcaoConta : ContentView
    {
        public CardOpcaoConta()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TituloProperty =
            BindableProperty.Create(nameof(Titulo), typeof(string), typeof(CardOpcaoConta), string.Empty);
        public string Titulo { get => (string)GetValue(TituloProperty); set => SetValue(TituloProperty, value); }

        public static readonly BindableProperty DescricaoProperty =
            BindableProperty.Create(nameof(Descricao), typeof(string), typeof(CardOpcaoConta), string.Empty);
        public string Descricao { get => (string)GetValue(DescricaoProperty); set => SetValue(DescricaoProperty, value); }

        public static readonly BindableProperty ValorProperty =
            BindableProperty.Create(nameof(Valor), typeof(TipoUsuario), typeof(CardOpcaoConta), TipoUsuario.Locatario);
        public TipoUsuario Valor { get => (TipoUsuario)GetValue(ValorProperty); set => SetValue(ValorProperty, value); }

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(CardOpcaoConta), false, propertyChanged: OnIsSelectedChanged);
        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }

        public static readonly BindableProperty SelecionarCommandProperty =
            BindableProperty.Create(nameof(SelecionarCommand), typeof(ICommand), typeof(CardOpcaoConta));
        public ICommand? SelecionarCommand { get => (ICommand?)GetValue(SelecionarCommandProperty); set => SetValue(SelecionarCommandProperty, value); }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            IconLocatario.IsVisible = Valor == TipoUsuario.Locatario;
            IconLocador.IsVisible = Valor == TipoUsuario.Locador;
        }

        private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (CardOpcaoConta)bindable;
            var selecionado = (bool)newValue;

            view.CardBorder.Stroke = selecionado ? (Brush)Application.Current!.Resources["AuthPrimaryBrush"] : new SolidColorBrush(Color.FromArgb("#DCDCDC"));
            view.CardBorder.StrokeThickness = selecionado ? 2 : 1;
            view.CardBorder.BackgroundColor = selecionado ? Color.FromArgb("#FFF9DB") : Color.FromArgb("#FAFAFA");
            view.IconBorder.BackgroundColor = selecionado ? (Color)Application.Current!.Resources["AuthPrimaryColor"] : Color.FromArgb("#EEEEEE");

            var iconColor = selecionado ? Colors.White : Color.FromArgb("#555555");
            view.IconLocatario.IconColor = iconColor;
            view.IconLocador.IconColor = iconColor;
        }

        private void OnCardTapped(object? sender, TappedEventArgs e) => SelecionarCommand?.Execute(Valor);
    }
}
