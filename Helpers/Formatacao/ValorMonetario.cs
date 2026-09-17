using System.Globalization;

namespace LOCATEM_DESKTOP.Helpers.Formatacao
{
    // Migrado de utils/Formatacao/valorMonetario.ts.
    public static class ValorMonetario
    {
        // Converte "45,00" -> 45. Aceita vírgula ou ponto como separador decimal.
        public static decimal ParaNumero(string? valorStr)
        {
            if (string.IsNullOrWhiteSpace(valorStr)) return 0;

            var normalizado = valorStr.Replace(".", string.Empty).Replace(',', '.');
            return decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out var numero)
                ? numero
                : 0;
        }

        //Formata para o padrão brasileiro, ex: 45 -> "R$ 45,00".
        public static string Formatar(decimal valor) =>
            $"R$ {valor.ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',')}";
    }
}
