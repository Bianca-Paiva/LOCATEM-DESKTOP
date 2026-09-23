using LOCATEM_DESKTOP.Helpers.Avaliacao;
using LOCATEM_DESKTOP.Helpers.Formatacao;
using LOCATEM_DESKTOP.Models.Ferramentas;
using LOCATEM_DESKTOP.Models.Locacoes;

namespace LOCATEM_DESKTOP.Services.Locacoes
{
    //
    // Locações mockadas — migrado de MinhasLocacoes.mock.ts. Cada locação referencia um produto real
    // do catálogo (ferramenta, imagem, locador vêm de lá) e acrescenta os dados da própria solicitação.
    // O valor é sempre calculado: diária x quantidade x nº de diárias.
    //
    public static class LocacoesMock
    {
        // Dados específicos de cada solicitação (o restante vem do produto).
        private record Solicitacao(
            string Id,
            int ProdutoId,
            StatusLocacao Status,
            string MensagemStatus,
            string DataInicio,
            string HoraInicio,
            string DataFim,
            string HoraFim,
            int Quantidade,
            string Locatario);

        private static readonly List<Solicitacao> Solicitacoes = new()
        {
            new("1", 1, StatusLocacao.Pendente, "A solicitação foi enviada e o locador ainda não respondeu", "15/07/2026", "09:00", "18/07/2026", "18:00", 1, "Carlos Andrade"),
            new("2", 2, StatusLocacao.AguardandoPagamento, "Locação aceita, efetue o pagamento em 24hs para continuar", "10/07/2026", "08:00", "12/07/2026", "17:00", 1, "Juliana Prado"),
            new("3", 3, StatusLocacao.PreparandoEntrega, "O pagamento foi confirmado e a entrega está sendo preparada", "05/07/2026", "09:00", "07/07/2026", "18:00", 1, "Rafael Lima"),
            new("4", 4, StatusLocacao.EmTransporte, "Ferramenta a caminho do seu endereço", "01/07/2026", "09:00", "03/07/2026", "18:00", 1, "Ana Souza"),
            new("5", 5, StatusLocacao.EmAndamento, "Você recebeu a ferramenta e o período de locação começou", "20/07/2026", "09:00", "25/07/2026", "18:00", 2, "Pedro Melo"),
            new("6", 6, StatusLocacao.AguardandoDevolucao, "O período de locação está acabando e deve retornar para o locador", "22/07/2026", "09:00", "24/07/2026", "18:00", 1, "Bianca Reis"),
            new("7", 7, StatusLocacao.DevolucaoEmTransporte, "A ferramenta foi coletada e está voltando para o locador", "22/07/2026", "09:00", "24/07/2026", "18:00", 1, "Marcos Vidal"),
            new("8", 6, StatusLocacao.Recusada, "Solicitação Recusada pelo Locador", "22/07/2026", "09:00", "24/07/2026", "18:00", 1, "Diego Farias"),
            new("9", 6, StatusLocacao.Cancelada, "Esta locação foi cancelada por você.", "22/07/2026", "09:00", "24/07/2026", "18:00", 1, "Sônia Alves"),
            new("10", 6, StatusLocacao.Finalizada, "Locação Finalizada", "22/07/2026", "09:00", "24/07/2026", "18:00", 1, "Fernando Lopes"),
            new("11", 6, StatusLocacao.Cancelada, "Locação cancelada automaticamente por falta de pagamento dentro do prazo.", "22/07/2026", "09:00", "24/07/2026", "18:00", 1, "Camila Torres"),
            new("12", 3, StatusLocacao.Pendente, "A solicitação foi enviada e o locador ainda não respondeu", "04/09/2026", "09:00", "07/09/2026", "18:00", 1, "Carlos Andrade"),
            new("13", 1, StatusLocacao.AguardandoPagamento, "Locação aceita, efetue o pagamento em 24hs para continuar", "06/09/2026", "09:00", "08/09/2026", "18:00", 1, "Renata Alves"),
            new("14", 9, StatusLocacao.Finalizada, "Locação Finalizada", "28/08/2026", "09:00", "31/08/2026", "18:00", 1, "Diego Martins"),
            new("15", 2, StatusLocacao.Pendente, "A solicitação foi enviada e o locador ainda não respondeu", "05/09/2026", "08:00", "06/09/2026", "17:00", 1, "Patrícia Nogueira"),
            new("16", 10, StatusLocacao.Finalizada, "Locação Finalizada", "20/08/2026", "09:00", "22/08/2026", "18:00", 1, "Fábio Ramos"),
        };

        public static List<Locacao> Criar(IReadOnlyCollection<Produto> produtos)
        {
            var locacoes = new List<Locacao>();

            foreach (var s in Solicitacoes)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == s.ProdutoId);
                if (produto is null) continue;

                var diarias = CalcularDiarias(s.DataInicio, s.DataFim);
                var valorTotal = ValorMonetario.ParaNumero(produto.Price) * s.Quantidade * diarias;

                locacoes.Add(new Locacao
                {
                    Id = s.Id,
                    ProdutoId = produto.Id,
                    Produto = produto.Title,
                    Imagem = produto.Images.FirstOrDefault() ?? string.Empty,
                    Periodo = FormatoDataBr.FormatarPeriodoBr(s.DataInicio, s.DataFim),
                    Locador = produto.Locador,
                    LocadorId = produto.LocadorId,
                    Locatario = s.Locatario,
                    Status = s.Status,
                    MensagemStatus = s.MensagemStatus,
                    DataInicio = s.DataInicio,
                    HoraInicio = s.HoraInicio,
                    DataFim = s.DataFim,
                    HoraFim = s.HoraFim,
                    Quantidade = s.Quantidade,
                    Valor = ValorMonetario.Formatar(valorTotal)
                });
            }

            return locacoes;
        }

        // Quantidade de diárias entre início e fim (mínimo de 1).
        private static int CalcularDiarias(string dataInicio, string dataFim)
        {
            var inicio = FormatoDataBr.ParaDataBr(dataInicio);
            var fim = FormatoDataBr.ParaDataBr(dataFim);
            if (inicio is null || fim is null) return 1;

            return Math.Max((int)Math.Round((fim.Value - inicio.Value).TotalDays), 1);
        }
    }
}
