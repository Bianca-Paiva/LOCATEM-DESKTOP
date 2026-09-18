using System.Text.RegularExpressions;

namespace LOCATEM_DESKTOP.Validation.Auth
{
    /// <summary>
    /// Validação e checksum de CNPJ — migrado do pacote "cpf-cnpj-validator" (cnpj.isValid)
    /// usado em validation/Cadastro/cadastroSchema.ts. Implementa o mesmo algoritmo de dígito verificador
    /// da Receita Federal usado por essa biblioteca.
    /// </summary>
    public static class DocumentValidator
    {
        public static bool IsValidCnpj(string? cnpj)
        {
            var d = OnlyDigits(cnpj);
            if (d.Length != 14 || AllDigitsEqual(d)) return false;

            var numeros = d[..12].Select(c => c - '0').ToArray();
            var dv1 = CalcularDigito(numeros, new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });
            var dv2 = CalcularDigito(numeros.Append(dv1).ToArray(), new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });

            return d[12] - '0' == dv1 && d[13] - '0' == dv2;
        }

        private static int CalcularDigito(int[] numeros, int[] pesos)
        {
            var soma = numeros.Select((n, i) => n * pesos[i]).Sum();
            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        private static bool AllDigitsEqual(string d) => d.Distinct().Count() == 1;

        private static string OnlyDigits(string? value) => Regex.Replace(value ?? string.Empty, @"\D", string.Empty);
    }
}