using System.Globalization;

namespace LOCATEM_DESKTOP.Helpers.Formatacao
{
   
    // Formatação de datas "dd/mm/aaaa" (formato em que DataInicio/DataFim circulam em Locacao).
    
    public static class FormatoDataBr
    {
        private static readonly string[] MesesAbrev =
            { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };

        // Converte "dd/mm/aaaa" em DateTime. Retorna null se vier vazio ou malformado.
        public static DateTime? ParaDataBr(string? dataBr)
        {
            if (string.IsNullOrWhiteSpace(dataBr)) return null;

            return DateTime.TryParseExact(dataBr, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var data)
                ? data
                : null;
        }

        // Formata "dd/mm/aaaa" para "dd Mon", ex: "23 Jul".
        public static string FormatarDiaMes(string? dataBr)
        {
            var data = ParaDataBr(dataBr);
            return data is null ? string.Empty : $"{data.Value.Day:00} {MesesAbrev[data.Value.Month - 1]}";
        }

        // Só o dia ("23") — usado na coluna de data da Agenda da Semana.
        public static string FormatarDia(string? dataBr)
        {
            var data = ParaDataBr(dataBr);
            return data is null ? string.Empty : $"{data.Value.Day:00}";
        }

        // Só o mês abreviado ("Jul") — usado na coluna de data da Agenda da Semana.
        public static string FormatarMes(string? dataBr)
        {
            var data = ParaDataBr(dataBr);
            return data is null ? string.Empty : MesesAbrev[data.Value.Month - 1];
        }

        // Período exibido nos cards de locação, ex: "15 Jul – 18 Jul 2026".
        public static string FormatarPeriodoBr(string dataInicioBr, string dataFimBr)
        {
            var inicio = ParaDataBr(dataInicioBr);
            var fim = ParaDataBr(dataFimBr);
            if (inicio is null || fim is null) return string.Empty;

            return $"{FormatarDiaMes(dataInicioBr)} – {FormatarDiaMes(dataFimBr)} {fim.Value.Year}";
        }
    }
}
