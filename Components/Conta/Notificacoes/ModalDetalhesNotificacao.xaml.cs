using System.Collections;
using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers.Notificacoes;
using LOCATEM_DESKTOP.Models.Notificacoes;
using MauiIcons.Material;

namespace LOCATEM_DESKTOP.Components.Conta.Notificacoes
{
    /// <summary>
    /// Modal reutilizável que apresenta os detalhes e ações de uma notificação selecionada.
    /// </summary>
    public partial class ModalDetalhesNotificacao : ContentView
    {
        /// <summary>Inicializa os elementos visuais do modal.</summary>
        public ModalDetalhesNotificacao()
        {
            InitializeComponent();
        }

        // Controla a exibição da camada de modal.
        public static readonly BindableProperty AbertoProperty =
            BindableProperty.Create(
                nameof(Aberto),
                typeof(bool),
                typeof(ModalDetalhesNotificacao),
                false);

        /// <summary>Indica se o modal está aberto.</summary>
        public bool Aberto
        {
            get => (bool)GetValue(AbertoProperty);
            set => SetValue(AbertoProperty, value);
        }

        // Registro selecionado que alimenta título, descrição, ícone e ações.
        public static readonly BindableProperty NotificacaoProperty =
            BindableProperty.Create(
                nameof(Notificacao),
                typeof(Notificacao),
                typeof(ModalDetalhesNotificacao),
                propertyChanged: OnNotificacaoChanged);

        /// <summary>Notificação atualmente apresentada.</summary>
        public Notificacao? Notificacao
        {
            get => (Notificacao?)GetValue(NotificacaoProperty);
            set => SetValue(NotificacaoProperty, value);
        }

        // Linhas já preparadas pelo ViewModel conforme a categoria da notificação.
        public static readonly BindableProperty LinhasDetalhesProperty =
            BindableProperty.Create(
                nameof(LinhasDetalhes),
                typeof(IEnumerable),
                typeof(ModalDetalhesNotificacao));

        /// <summary>Coleção de rótulos e valores exibidos no bloco de detalhes.</summary>
        public IEnumerable? LinhasDetalhes
        {
            get => (IEnumerable?)GetValue(LinhasDetalhesProperty);
            set => SetValue(LinhasDetalhesProperty, value);
        }

        // Controla a presença do botão Renovar.
        public static readonly BindableProperty MostrarRenovarProperty =
            BindableProperty.Create(
                nameof(MostrarRenovar),
                typeof(bool),
                typeof(ModalDetalhesNotificacao),
                false,
                propertyChanged: OnVisibilidadeAcoesChanged);

        /// <summary>Indica se o botão Renovar deve ser mostrado.</summary>
        public bool MostrarRenovar
        {
            get => (bool)GetValue(MostrarRenovarProperty);
            set => SetValue(MostrarRenovarProperty, value);
        }

        // Controla a presença do botão de ação contextual.
        public static readonly BindableProperty MostrarAcaoProperty =
            BindableProperty.Create(
                nameof(MostrarAcao),
                typeof(bool),
                typeof(ModalDetalhesNotificacao),
                false,
                propertyChanged: OnVisibilidadeAcoesChanged);

        /// <summary>Indica se a ação contextual possui destino implementado.</summary>
        public bool MostrarAcao
        {
            get => (bool)GetValue(MostrarAcaoProperty);
            set => SetValue(MostrarAcaoProperty, value);
        }

        // Texto configurado pelo ViewModel para a ação contextual.
        public static readonly BindableProperty TextoAcaoProperty =
            BindableProperty.Create(
                nameof(TextoAcao),
                typeof(string),
                typeof(ModalDetalhesNotificacao),
                string.Empty);

        /// <summary>Rótulo exibido no botão contextual.</summary>
        public string TextoAcao
        {
            get => (string)GetValue(TextoAcaoProperty);
            set => SetValue(TextoAcaoProperty, value);
        }

        // Comando acionado pelo botão X do cabeçalho.
        public static readonly BindableProperty FecharCommandProperty =
            BindableProperty.Create(
                nameof(FecharCommand),
                typeof(ICommand),
                typeof(ModalDetalhesNotificacao));

        /// <summary>Comando responsável por fechar o modal.</summary>
        public ICommand? FecharCommand
        {
            get => (ICommand?)GetValue(FecharCommandProperty);
            set => SetValue(FecharCommandProperty, value);
        }

        // Comando compartilhado com o botão Renovar do card.
        public static readonly BindableProperty RenovarCommandProperty =
            BindableProperty.Create(
                nameof(RenovarCommand),
                typeof(ICommand),
                typeof(ModalDetalhesNotificacao));

        /// <summary>Comando responsável pela renovação/removal da notificação.</summary>
        public ICommand? RenovarCommand
        {
            get => (ICommand?)GetValue(RenovarCommandProperty);
            set => SetValue(RenovarCommandProperty, value);
        }

        // Comando que navega para a próxima etapa natural do fluxo.
        public static readonly BindableProperty AcaoCommandProperty =
            BindableProperty.Create(
                nameof(AcaoCommand),
                typeof(ICommand),
                typeof(ModalDetalhesNotificacao));

        /// <summary>Comando do botão contextual do modal.</summary>
        public ICommand? AcaoCommand
        {
            get => (ICommand?)GetValue(AcaoCommandProperty);
            set => SetValue(AcaoCommandProperty, value);
        }

        // Propriedade derivada usada somente pelo XAML para esconder o rodapé vazio.
        public bool MostrarRodape => MostrarRenovar || MostrarAcao;

        /// <summary>
        /// Atualiza ícone, cores e ícone do botão contextual quando a seleção muda.
        /// </summary>
        private static void OnNotificacaoChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            var modal = (ModalDetalhesNotificacao)bindable;
            modal.AplicarVisual(newValue as Notificacao);
        }

        /// <summary>
        /// Notifica o XAML quando alguma ação altera a visibilidade do rodapé.
        /// </summary>
        private static void OnVisibilidadeAcoesChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            ((ModalDetalhesNotificacao)bindable).OnPropertyChanged(nameof(MostrarRodape));
        }

        /// <summary>
        /// Aplica a mesma configuração visual do card e escolhe um ícone coerente para a ação principal.
        /// </summary>
        private void AplicarVisual(Notificacao? notificacao)
        {
            if (notificacao is null)
                return;

            // Mantém o ícone principal sincronizado com NotificacaoCard.
            var visual = NotificacaoVisualHelper.Obter(notificacao);
            IconeNotificacao.Icon = visual.Icone;
            IconeNotificacao.IconColor = Color.FromArgb(visual.CorIcone);
            IconeContainer.BackgroundColor = Color.FromArgb(visual.CorFundo);

            // A ação contextual usa ícones já disponíveis na biblioteca Material instalada no projeto.
            IconeAcao.Icon = notificacao.Categoria switch
            {
                CategoriaNotificacao.PagamentoPendente => MaterialIcons.CreditCard,
                CategoriaNotificacao.PagamentoRecusado => MaterialIcons.CreditCard,
                CategoriaNotificacao.AvaliacaoPendente => MaterialIcons.Star,
                _ => MaterialIcons.Assignment
            };
        }
    }
}
