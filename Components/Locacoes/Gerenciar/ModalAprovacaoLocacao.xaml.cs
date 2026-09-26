using System.Windows.Input;
using LOCATEM_DESKTOP.Models.Locacoes;

namespace LOCATEM_DESKTOP.Components.Locacoes.Gerenciar
{
    /// <summary>Modal de aprovação/recusa usado somente para solicitações pendentes.</summary>
    public partial class ModalAprovacaoLocacao : ContentView
    {
        public ModalAprovacaoLocacao()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty IsOpenProperty =
            BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(ModalAprovacaoLocacao), false);

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly BindableProperty LocacaoProperty =
            BindableProperty.Create(
                nameof(Locacao),
                typeof(Locacao),
                typeof(ModalAprovacaoLocacao),
                propertyChanged: (bindable, _, novoValor) =>
                    ((ModalAprovacaoLocacao)bindable).AtualizarEndereco(novoValor as Locacao));

        public Locacao? Locacao
        {
            get => (Locacao?)GetValue(LocacaoProperty);
            set => SetValue(LocacaoProperty, value);
        }

        public static readonly BindableProperty ApproveCommandProperty =
            BindableProperty.Create(nameof(ApproveCommand), typeof(ICommand), typeof(ModalAprovacaoLocacao));

        public ICommand? ApproveCommand
        {
            get => (ICommand?)GetValue(ApproveCommandProperty);
            set => SetValue(ApproveCommandProperty, value);
        }

        public static readonly BindableProperty RejectCommandProperty =
            BindableProperty.Create(nameof(RejectCommand), typeof(ICommand), typeof(ModalAprovacaoLocacao));

        public ICommand? RejectCommand
        {
            get => (ICommand?)GetValue(RejectCommandProperty);
            set => SetValue(RejectCommandProperty, value);
        }

        public static readonly BindableProperty CloseCommandProperty =
            BindableProperty.Create(nameof(CloseCommand), typeof(ICommand), typeof(ModalAprovacaoLocacao));

        public ICommand? CloseCommand
        {
            get => (ICommand?)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        private void AtualizarEndereco(Locacao? locacao)
        {
            EnderecoLabel.Text = locacao?.Endereco?.TextoExibicao ?? "Endereço não informado";
        }
    }
}
