using System.Globalization;
using LOCATEM_DESKTOP.Models.Locacoes;

namespace LOCATEM_DESKTOP.Helpers.Locacoes
{
    public record AcaoLocacaoTexto(string Titulo, string Descricao);

    /// <summary>
    /// Regras de apresentação do card de Gerenciar Locações, migradas de
    /// utils/Locacoes/timeLineLocacao.ts e useGerenciarLocacoes.ts.
    /// </summary>
    public static class LocacaoGerenciamentoHelper
    {
        public static bool EhEncerrada(StatusLocacao status) =>
            status is StatusLocacao.Finalizada or StatusLocacao.Recusada or StatusLocacao.Cancelada;

        public static string? ObterChaveFiltro(StatusLocacao status) => status switch
        {
            StatusLocacao.Pendente => "pendente",
            StatusLocacao.AguardandoPagamento => "aguardandoPagamento",
            StatusLocacao.Confirmada or StatusLocacao.PreparandoEntrega => "preparandoEntrega",
            StatusLocacao.EmTransporte => "emTransporte",
            StatusLocacao.EmAndamento => "emAndamento",
            StatusLocacao.AguardandoDevolucao => "aguardandoDevolucao",
            StatusLocacao.DevolucaoEmTransporte => "devolucaoEmTransporte",
            _ => null
        };

        public static string FormatarHorarioEntrega(Locacao locacao)
        {
            if (!TryParseData(locacao.DataInicio, out var data) || string.IsNullOrWhiteSpace(locacao.HoraInicio))
                return "—";

            return $"{FormatarDiaMesAno(data)} às {locacao.HoraInicio}";
        }

        public static string FormatarJanelaDevolucao(Locacao locacao)
        {
            if (!TryParseData(locacao.DataFim, out var data) || string.IsNullOrWhiteSpace(locacao.HoraFim))
                return "—";

            return $"{FormatarDiaMesAno(data)}, das {FormatarIntervaloHorario(locacao.HoraFim)}";
        }

        public static AcaoLocacaoTexto ObterAcaoAgora(Locacao locacao) => locacao.Status switch
        {
            StatusLocacao.Pendente => new("Analise a solicitação", "Aprove ou recuse o pedido do locatário."),
            StatusLocacao.AguardandoPagamento => new("Aguarde a confirmação do pagamento", "Assim que o locatário pagar, você poderá preparar a entrega."),
            StatusLocacao.Confirmada or StatusLocacao.PreparandoEntrega => new("Prepare a ferramenta para a entrega", "Deixe o equipamento pronto e disponível no horário combinado."),
            StatusLocacao.EmTransporte => new("Acompanhe a entrega", "A ferramenta está a caminho do locatário — acompanhe até a chegada."),
            StatusLocacao.EmAndamento => new("Nenhuma ação necessária no momento", "Aguarde o fim do período de uso para receber a devolução."),
            StatusLocacao.AguardandoDevolucao => new("Fique disponível para receber a ferramenta", "Esteja no endereço combinado no horário definido para a devolução."),
            StatusLocacao.DevolucaoEmTransporte => new("Aguarde o recebimento da ferramenta", "A devolução está a caminho — aguarde a chegada do equipamento."),
            StatusLocacao.Finalizada => new("Locação concluída", "Nenhuma ação é necessária."),
            StatusLocacao.Recusada => new("Solicitação recusada", "Nenhuma ação é necessária."),
            StatusLocacao.Cancelada => new("Locação cancelada", "Nenhuma ação é necessária."),
            _ => new("Acompanhe a locação", locacao.MensagemStatus)
        };

        public static AcaoLocacaoTexto ObterProximaEtapa(Locacao locacao) => locacao.Status switch
        {
            StatusLocacao.Pendente => new("Aguardando pagamento", "Após aprovar, o locatário terá um prazo para pagar."),
            StatusLocacao.AguardandoPagamento => new("Preparando entrega", "Assim que o pagamento for confirmado."),
            StatusLocacao.Confirmada or StatusLocacao.PreparandoEntrega => new("Em transporte", "Quando a entrega for iniciada."),
            StatusLocacao.EmTransporte => new("Em andamento", "Quando a ferramenta chegar ao locatário."),
            StatusLocacao.EmAndamento => new("Aguardando devolução", "Ao fim do período de uso contratado."),
            StatusLocacao.AguardandoDevolucao => new("Devolução em transporte", "Quando o locatário enviar a ferramenta de volta."),
            StatusLocacao.DevolucaoEmTransporte => new("Finalizada", "Quando você confirmar o recebimento da ferramenta."),
            StatusLocacao.Finalizada => new("Concluída", "Não há próximas etapas."),
            StatusLocacao.Recusada => new("Sem próximas etapas", "Esta solicitação foi recusada."),
            StatusLocacao.Cancelada => new("Sem próximas etapas", "Esta locação foi cancelada."),
            _ => new("—", string.Empty)
        };

        private static bool TryParseData(string valor, out DateTime data) =>
            DateTime.TryParseExact(valor, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out data);

        private static string FormatarDiaMesAno(DateTime data)
        {
            var meses = new[] { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
            return $"{data:dd} {meses[data.Month - 1]} {data:yyyy}";
        }

        private static string FormatarIntervaloHorario(string horarioInicio)
        {
            if (!TimeSpan.TryParseExact(horarioInicio, @"hh\:mm", CultureInfo.InvariantCulture, out var inicio))
                return horarioInicio;

            var fim = inicio.Add(TimeSpan.FromHours(3));
            return $"{inicio:hh\\:mm} às {fim:hh\\:mm}";
        }
    }
}
