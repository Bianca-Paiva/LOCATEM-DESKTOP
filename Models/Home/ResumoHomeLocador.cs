namespace LOCATEM_DESKTOP.Models.Home
{
    /// <summary>Números exibidos nos cards de resumo do topo da Home do Locador.</summary>
    public class ResumoHomeLocador
    {
        public int FerramentasAtivas { get; set; }

        /// <summary>Ferramentas cadastradas no mês atual — alimenta o texto "+N este mês".</summary>
        public int FerramentasCadastradasEsteMes { get; set; }

        public int LocacoesEmAndamento { get; set; }
        public int SolicitacoesPendentes { get; set; }

        /// <summary>Soma das locações finalizadas com início no mês atual.</summary>
        public decimal FaturamentoMesAtual { get; set; }
        public decimal FaturamentoMesAnterior { get; set; }
    }
}
