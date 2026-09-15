namespace LOCATEM_DESKTOP.Models.Auth
{
    /// <summary>
    /// Indicadores de reputação exibidos no card "Reputação" da tela de Perfil.
    /// Migrado de types/Auth/usuario.types.ts (ReputacaoUsuario).
    /// </summary>
    public class ReputacaoUsuario
    {
        public double Rating { get; set; }
        public int TotalAvaliacoes { get; set; }
        public int LocacoesConcluidas { get; set; }

        /// <summary>Só se aplica a Locadores (indicador "entregas no prazo" do protótipo).</summary>
        public double? EntregasNoPrazoPercentual { get; set; }
    }
}
