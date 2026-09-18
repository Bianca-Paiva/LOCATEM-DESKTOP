using MauiIcons.Material;
using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Components.Ferramentas
{
    /// <summary>Configuração visual/textual de cada status de ferramenta. Migrado de statusFerramentaConfig.ts.</summary>
    public record StatusFerramentaVisual(string Label, MaterialIcons Icone, string Cor, string Borda, string Fundo);

    public static class StatusFerramentaConfig
    {
        private static readonly Dictionary<StatusFerramenta, StatusFerramentaVisual> Config = new()
        {
            [StatusFerramenta.Disponivel] = new("Disponível", MaterialIcons.CheckCircle, "#137333", "#40137333", "#E6F4EA"),
            [StatusFerramenta.Locada] = new("Locada", MaterialIcons.Inventory, "#005D75", "#40005D75", "#EAF6FF"),
            [StatusFerramenta.Indisponivel] = new("Indisponível", MaterialIcons.Block, "#546E7A", "#B0BEC5", "#ECEFF1"),
            [StatusFerramenta.Manutencao] = new("Em manutenção", MaterialIcons.Build, "#A74B00", "#40A74B00", "#FFEBCF")
        };

        public static StatusFerramentaVisual Obter(StatusFerramenta status) => Config[status];
    }
}
