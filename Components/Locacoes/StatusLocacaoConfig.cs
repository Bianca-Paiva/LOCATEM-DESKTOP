using MauiIcons.Material;
using LOCATEM_DESKTOP.Models.Locacoes;

namespace LOCATEM_DESKTOP.Components.Locacoes
{
    /// <summary>Configuração visual/textual de cada status de locação. Migrado de EtiquetaStatus/statusConfig.ts.</summary>
    public record StatusLocacaoVisual(string Label, MaterialIcons Icone, string Cor, string Borda, string Fundo);

    public static class StatusLocacaoConfig
    {
        private static readonly Dictionary<StatusLocacao, StatusLocacaoVisual> Config = new()
        {
            [StatusLocacao.Pendente] = new("Aguardando aprovação", MaterialIcons.Schedule, "#7A5A00", "#407A5A00", "#FFF4DD"),
            [StatusLocacao.AguardandoPagamento] = new("Aguardando pagamento", MaterialIcons.CreditCard, "#A74B00", "#40A74B00", "#FFEBCF"),
            [StatusLocacao.Confirmada] = new("Confirmada", MaterialIcons.Check, "#137333", "#40137333", "#E6F4EA"),
            [StatusLocacao.PreparandoEntrega] = new("Preparando entrega", MaterialIcons.Inventory, "#005D75", "#40005D75", "#EAF6FF"),
            [StatusLocacao.EmTransporte] = new("Em transporte", MaterialIcons.LocalShipping, "#005D75", "#40005D75", "#EAF6FF"),
            [StatusLocacao.EmAndamento] = new("Em andamento", MaterialIcons.Build, "#005D75", "#40005D75", "#EAF6FF"),
            [StatusLocacao.AguardandoDevolucao] = new("Aguardando devolução", MaterialIcons.AssignmentReturn, "#005D75", "#40005D75", "#EAF6FF"),
            [StatusLocacao.DevolucaoEmTransporte] = new("Devolução em transporte", MaterialIcons.LocalShipping, "#005D75", "#40005D75", "#EAF6FF"),
            [StatusLocacao.Finalizada] = new("Finalizada", MaterialIcons.Check, "#137333", "#40137333", "#E6F4EA"),
            [StatusLocacao.Recusada] = new("Recusada", MaterialIcons.Block, "#BA1A1A", "#40BA1A1A", "#FFDAD6"),
            [StatusLocacao.Cancelada] = new("Cancelada", MaterialIcons.Close, "#546E7A", "#B0BEC5", "#ECEFF1")
        };

        public static StatusLocacaoVisual Obter(StatusLocacao status) => Config[status];
    }
}
