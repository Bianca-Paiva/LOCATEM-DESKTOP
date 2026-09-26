using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers.Notificacoes;
using LOCATEM_DESKTOP.Models.Notificacoes;

namespace LOCATEM_DESKTOP.Components.Conta.Notificacoes
{
    /// <summary>
    /// Card visual de uma notificação.
    /// O componente cuida somente de aparência e encaminha as ações para o ViewModel da página.
    /// </summary>
    public partial class NotificacaoCard : ContentView
    {
        /// <summary>
        /// Inicializa o componente e acompanha mudanças de largura para preservar o rodapé responsivo.
        /// </summary>
        public NotificacaoCard()
        {
            InitializeComponent();

            // Ajusta o rodapé quando o card é redimensionado em uma janela menor.
            SizeChanged += (_, _) => AtualizarResponsividade();
        }

        // Propriedade que recebe o registro renderizado pelo BindableLayout da página.
        public static readonly BindableProperty NotificacaoProperty =
            BindableProperty.Create(
                nameof(Notificacao),
                typeof(Notificacao),
                typeof(NotificacaoCard),
                propertyChanged: OnNotificacaoChanged);

        /// <summary>Notificação exibida pelo card.</summary>
        public Notificacao? Notificacao
        {
            get => (Notificacao?)GetValue(NotificacaoProperty);
            set => SetValue(NotificacaoProperty, value);
        }

        // Comando recebido do ViewModel para remover uma notificação renovável.
        public static readonly BindableProperty RenovarCommandProperty =
            BindableProperty.Create(
                nameof(RenovarCommand),
                typeof(ICommand),
                typeof(NotificacaoCard));

        /// <summary>Comando do botão Renovar.</summary>
        public ICommand? RenovarCommand
        {
            get => (ICommand?)GetValue(RenovarCommandProperty);
            set => SetValue(RenovarCommandProperty, value);
        }

        // Comando recebido do ViewModel para abrir o modal de detalhes.
        public static readonly BindableProperty AbrirDetalhesCommandProperty =
            BindableProperty.Create(
                nameof(AbrirDetalhesCommand),
                typeof(ICommand),
                typeof(NotificacaoCard));

        /// <summary>Comando do botão Ver detalhes.</summary>
        public ICommand? AbrirDetalhesCommand
        {
            get => (ICommand?)GetValue(AbrirDetalhesCommandProperty);
            set => SetValue(AbrirDetalhesCommandProperty, value);
        }

        /// <summary>
        /// Recalcula ícone e cores sempre que o card recebe outra notificação.
        /// </summary>
        private static void OnNotificacaoChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            var card = (NotificacaoCard)bindable;
            card.AplicarVisual(newValue as Notificacao);
        }

        /// <summary>
        /// Usa o helper central para manter cards e modal com a mesma identidade visual.
        /// </summary>
        private void AplicarVisual(Notificacao? notificacao)
        {
            if (notificacao is null)
                return;

            // Status ligados a locação reutilizam StatusLocacaoConfig automaticamente pelo helper.
            var visual = NotificacaoVisualHelper.Obter(notificacao);

            // Aplica o ícone Material correspondente.
            IconeNotificacao.Icon = visual.Icone;

            // Aplica a cor do desenho do ícone.
            IconeNotificacao.IconColor = Color.FromArgb(visual.CorIcone);

            // Aplica a cor de fundo do círculo do ícone.
            IconeContainer.BackgroundColor = Color.FromArgb(visual.CorFundo);
        }

        /// <summary>
        /// Em larguras menores remove o recuo do rodapé para liberar espaço aos botões.
        /// </summary>
        private void AtualizarResponsividade()
        {
            if (Width <= 0)
                return;

            // O Web remove o padding esquerdo do footer abaixo de 480px; aqui usamos 620 para Desktop estreito.
            Rodape.Margin = Width < 620
                ? Thickness.Zero
                : new Thickness(52, 0, 0, 0);
        }
    }
}
