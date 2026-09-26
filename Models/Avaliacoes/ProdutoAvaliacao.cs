using System.ComponentModel;
using System.Runtime.CompilerServices;
using MauiIcons.Material;

namespace LOCATEM_DESKTOP.Models.Avaliacoes
{
    /// <summary>Estado visual de uma avaliação derivado da existência ou não de um registro salvo.</summary>
    public enum StatusAvaliacao
    {
        Pendente,
        Realizada
    }

    /// <summary>
    /// Item exibido na lista de avaliações.
    /// Ele representa uma locação finalizada, e não um produto independente, preservando o vínculo correto do Web.
    /// </summary>
    public sealed class ProdutoAvaliacao : INotifyPropertyChanged
    {
        private int _notaGlobal;

        /// <summary>ID da locação que originou a avaliação.</summary>
        public string LocacaoId { get; init; } = string.Empty;

        /// <summary>Nome da ferramenta locada.</summary>
        public string Nome { get; init; } = string.Empty;

        /// <summary>Texto de período já preparado para apresentação.</summary>
        public string DataLocacao { get; init; } = string.Empty;

        /// <summary>Imagem da ferramenta no catálogo.</summary>
        public string Imagem { get; init; } = string.Empty;

        /// <summary>Nome da loja dona da ferramenta.</summary>
        public string LojaNome { get; init; } = string.Empty;

        /// <summary>Logo da loja quando existir no conjunto de assets do Desktop.</summary>
        public string? LojaLogo { get; init; }

        /// <summary>Nome do locatário avaliado pelo locador.</summary>
        public string Locatario { get; init; } = string.Empty;

        /// <summary>Define se o item ainda precisa ser avaliado ou se já possui registro salvo.</summary>
        public StatusAvaliacao Status { get; init; }

        /// <summary>Controla se as estrelas do card podem iniciar uma nova avaliação diretamente.</summary>
        public bool PodeAvaliar => Status == StatusAvaliacao.Pendente;

        /// <summary>Indica a existência de logo para alternar entre imagem e fallback visual.</summary>
        public bool TemLogo => !string.IsNullOrWhiteSpace(LojaLogo);

        /// <summary>Nota global mostrada no card e atualizada durante a interação do usuário.</summary>
        public int NotaGlobal
        {
            get => _notaGlobal;
            set
            {
                if (_notaGlobal == value)
                    return;

                _notaGlobal = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NotaGlobal)));
            }
        }

        /// <summary>Notifica o XAML quando a nota global muda.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    /// <summary>
    /// Linha de avaliação apresentada dentro do modal.
    /// Cada instância controla uma nota específica e seu estado de validação.
    /// </summary>
    public sealed class AvaliacaoAspectoItem : INotifyPropertyChanged
    {
        private int _nota;
        private bool _temErro;

        /// <summary>Chave usada para salvar a nota no registro final.</summary>
        public AspectoAvaliacao Aspecto { get; init; }

        /// <summary>Rótulo equivalente ao apresentado no modal Web.</summary>
        public string Label { get; init; } = string.Empty;

        /// <summary>Ícone Material associado ao aspecto.</summary>
        public MaterialIcons Icone { get; init; }

        /// <summary>Nota atual entre zero e cinco.</summary>
        public int Nota
        {
            get => _nota;
            set
            {
                if (_nota == value)
                    return;

                _nota = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Sinaliza visualmente um aspecto obrigatório ainda não preenchido.</summary>
        public bool TemErro
        {
            get => _temErro;
            set
            {
                if (_temErro == value)
                    return;

                _temErro = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Evento utilizado pelo binding da linha do modal.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Dispara a atualização apenas da propriedade modificada.</summary>
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Parâmetro emitido pelo componente de estrelas.
    /// Contexto identifica o item afetado e Nota informa o valor selecionado.
    /// </summary>
    public sealed record SelecaoEstrela(object? Contexto, int Nota);
}
