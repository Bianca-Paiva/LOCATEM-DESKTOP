using LOCATEM_DESKTOP.Models.Locacoes;

namespace LOCATEM_DESKTOP.Models.Notificacoes
{
    /// <summary>
    /// Tipos visuais usados quando a notificação não está ligada a um status de locação.
    /// Os valores equivalem aos tipos usados na implementação React.
    /// </summary>
    public enum TipoNotificacao
    {
        Sucesso,
        Aviso,
        Entrega,
        Erro,
        Informacao,
        Promocao,
        Mensagem,
        Lembrete
    }

    /// <summary>
    /// Identifica o assunto da notificação e define quais detalhes e ações são exibidos.
    /// </summary>
    public enum CategoriaNotificacao
    {
        LocacaoConfirmada,
        LocacaoCancelada,
        DevolucaoPendente,
        DevolucaoAtrasada,
        EntregaAndamento,
        EntregaConcluida,
        FerramentaDevolvida,
        PagamentoPendente,
        PagamentoConfirmado,
        PagamentoRecusado,
        PromocaoDisponivel,
        AvaliacaoPendente,
        NovaMensagem
    }

    /// <summary>
    /// Períodos disponíveis no filtro da tela, preservando as mesmas opções do Web.
    /// </summary>
    public enum FiltroNotificacao
    {
        Todas,
        Hoje,
        Ontem,
        EstaSemana,
        EsteMes
    }

    /// <summary>
    /// Campos opcionais exibidos no modal de detalhes.
    /// Cada categoria utiliza somente os dados que fazem sentido para o seu fluxo.
    /// </summary>
    public sealed class DetalhesNotificacao
    {
        public string? Equipamento { get; init; }
        public string? Status { get; init; }
        public string? DataConfirmacao { get; init; }
        public string? PeriodoLocacao { get; init; }
        public string? Valor { get; init; }
        public string? FormaPagamento { get; init; }
        public string? DataLimite { get; init; }
        public string? StatusEntrega { get; init; }
        public string? PrevisaoChegada { get; init; }
        public string? DataDevolucao { get; init; }
        public string? StatusPagamento { get; init; }
        public string? MotivoCancelamento { get; init; }
        public string? DataCancelamento { get; init; }
        public string? ValorReembolso { get; init; }
        public string? DiasAtraso { get; init; }
        public string? Multa { get; init; }
        public string? DataEntrega { get; init; }
        public string? RecebidoPor { get; init; }
        public string? MotivoRecusa { get; init; }
        public string? Cupom { get; init; }
        public string? Desconto { get; init; }
        public string? Validade { get; init; }
        public string? CategoriaEquipamento { get; init; }
        public string? NotaSugerida { get; init; }
        public string? Remetente { get; init; }
        public string? Assunto { get; init; }
        public string? Mensagem { get; init; }
    }

    /// <summary>
    /// Modelo principal da tela de Notificações.
    /// Mantém conteúdo, data de filtro, vínculo opcional com locação e dados do modal.
    /// </summary>
    public sealed class Notificacao
    {
        public string Id { get; init; } = string.Empty;
        public TipoNotificacao Tipo { get; init; }
        public CategoriaNotificacao Categoria { get; init; }
        public string Titulo { get; init; } = string.Empty;
        public string Descricao { get; init; } = string.Empty;
        public string Timestamp { get; init; } = string.Empty;
        public DateTime Data { get; init; }
        public string? InformacaoExtra { get; init; }

        /// <summary>Controla a linha extra sem depender de conversores no XAML.</summary>
        public bool TemInformacaoExtra => !string.IsNullOrWhiteSpace(InformacaoExtra);

        /// <summary>Controla a exibição do horário sem depender de conversores no XAML.</summary>
        public bool TemTimestamp => !string.IsNullOrWhiteSpace(Timestamp);

        public bool MostrarRenovar { get; init; }
        public DetalhesNotificacao Detalhes { get; init; } = new();

        /// <summary>
        /// Quando preenchido, o card reutiliza cor e ícone do status já existente no Desktop.
        /// </summary>
        public StatusLocacao? StatusLocacao { get; init; }

        /// <summary>
        /// Identificador da locação relacionada, usado somente em ações contextuais.
        /// </summary>
        public string? LocacaoId { get; init; }
    }

    /// <summary>
    /// Linha simples apresentada dentro do bloco de detalhes do modal.
    /// </summary>
    public sealed class LinhaDetalheNotificacao
    {
        public string Rotulo { get; init; } = string.Empty;
        public string Valor { get; init; } = string.Empty;
    }

    /// <summary>
    /// Item numérico da paginação, com estado próprio para destacar a página atual.
    /// </summary>
    public sealed class PaginaNotificacao
    {
        public int Numero { get; init; }
        public bool Ativa { get; init; }
    }
}
