using MauiIcons.Material;
using MauiIcons.Material.Outlined;
using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Components.Ferramentas
{
    /// <summary>
    /// Configuração visual/textual de cada status de ferramenta.
    ///
    /// Label    = texto do selo do card (ex.: "Disponível").
    /// TabLabel = texto da aba de filtro em Minhas Ferramentas (ex.: "Disponíveis"),
    ///            equivalente ao tabLabel de statusFerramentaConfig.ts.
    /// </summary>
    public record StatusFerramentaVisual(
        string Label,
        string TabLabel,
        Enum Icone,
        string Cor,
        string Borda,
        string Fundo
    );

    public static class StatusFerramentaConfig
    {
        private static readonly Dictionary<StatusFerramenta, StatusFerramentaVisual> Config = new()
        {
            // ── Disponível para locação ────────────────────────────────
            [StatusFerramenta.Disponivel] = new(
                "Disponível",
                "Disponíveis",
                MaterialIcons.CheckCircle,
                "#137333",
                "#40137333",
                "#E6F4EA"
            ),

            // ── Em uso ────────────────────────────────────────────────
            [StatusFerramenta.Locada] = new(
                "Locada",
                "Locadas",
                MaterialOutlinedIcons.Inventory,
                "#005D75",
                "#40005D75",
                "#EAF6FF"
            ),

            // ── Temporariamente indisponível ───────────────────────────
            [StatusFerramenta.Manutencao] = new(
                "Em manutenção",
                "Em manutenção",
                MaterialIcons.Build,
                "#A74B00",
                "#40A74B00",
                "#FFEBCF"
            ),

            // ── Indisponível ──────────────────────────────────────────
            [StatusFerramenta.Indisponivel] = new(
                "Indisponível",
                "Indisponíveis",
                MaterialIcons.Block,
                "#546E7A",
                "#B0BEC5",
                "#ECEFF1"
            )
        };

        public static StatusFerramentaVisual Obter(StatusFerramenta status)
            => Config[status];
    }
}