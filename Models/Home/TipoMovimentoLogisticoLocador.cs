namespace LOCATEM_DESKTOP.Models.Home
{
    /// <summary>
    /// Movimento logístico exibido na "Agenda da Semana". Migrado de HomeLocador.types.ts.
    ///
    /// A entrega é feita por transportadora terceirizada, então nunca usamos "Retirada"/"Devolução"
    /// (ambíguos quanto a quem está com a ferramenta):
    /// - ColetaParaEntrega: LOCADOR -> transportadora -> LOCATÁRIO.
    /// - RetornoAoLocador: LOCATÁRIO -> transportadora -> LOCADOR.
    /// </summary>
    public enum TipoMovimentoLogisticoLocador
    {
        ColetaParaEntrega,
        RetornoAoLocador
    }
}
