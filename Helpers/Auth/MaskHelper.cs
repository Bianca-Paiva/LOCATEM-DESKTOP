using System.Text;
using System.Text.RegularExpressions;

namespace LOCATEM_DESKTOP.Helpers.Auth
{
    /// <summary>
    /// As implementações abaixo seguem o padrão-mercado de máscara progressiva brasileira
    /// (o mesmo formato de saída usado nos placeholders da tela: "000.000.000-00",
    /// "00.000.000/0000-00", "(11) 91234-5678", "00000-000") e a validação de telefone
    /// segue a mesma regra prática usada pela libphonenumber-js para números BR (DDD 11-99 e
    /// 8 ou 9 dígitos, começando com 9 quando tiver 9 dígitos).
    /// </summary>
    public static class MaskHelper
    {
        private static string OnlyDigits(string value) => Regex.Replace(value ?? string.Empty, @"\D", string.Empty);

        /// <summary>000.000.000-00</summary>
        public static string MaskCpf(string value)
        {
            var d = OnlyDigits(value);
            if (d.Length > 11) d = d[..11];

            var sb = new StringBuilder();
            for (int i = 0; i < d.Length; i++)
            {
                if (i == 3 || i == 6) sb.Append('.');
                if (i == 9) sb.Append('-');
                sb.Append(d[i]);
            }
            return sb.ToString();
        }

        /// <summary>00.000.000/0000-00</summary>
        public static string MaskCnpj(string value)
        {
            var d = OnlyDigits(value);
            if (d.Length > 14) d = d[..14];

            var sb = new StringBuilder();
            for (int i = 0; i < d.Length; i++)
            {
                if (i == 2 || i == 5) sb.Append('.');
                if (i == 8) sb.Append('/');
                if (i == 12) sb.Append('-');
                sb.Append(d[i]);
            }
            return sb.ToString();
        }

        /// <summary>(11) 91234-5678 ou (11) 1234-5678, dependendo da quantidade de dígitos.</summary>
        public static string MaskPhone(string value)
        {
            var d = OnlyDigits(value);
            if (d.Length > 11) d = d[..11];

            if (d.Length == 0) return string.Empty;

            var sb = new StringBuilder("(");
            for (int i = 0; i < d.Length; i++)
            {
                if (i == 2) sb.Append(") ");
                if ((d.Length > 10 && i == 7) || (d.Length <= 10 && i == 6)) sb.Append('-');
                sb.Append(d[i]);
            }
            return sb.ToString();
        }

        /// <summary>00000-000</summary>
        public static string MaskCep(string value)
        {
            var d = OnlyDigits(value);
            if (d.Length > 8) d = d[..8];

            var sb = new StringBuilder();
            for (int i = 0; i < d.Length; i++)
            {
                if (i == 5) sb.Append('-');
                sb.Append(d[i]);
            }
            return sb.ToString();
        }

        /// <summary>Telefone válido com DDD: 10 ou 11 dígitos, DDD entre 11-99.</summary>
        public static bool ValidatePhone(string value)
        {
            var d = OnlyDigits(value);
            if (d.Length != 10 && d.Length != 11) return false;

            var ddd = int.Parse(d[..2]);
            if (ddd < 11 || ddd > 99) return false;

            // Celular com 9 dígitos deve começar com 9.
            if (d.Length == 11 && d[2] != '9') return false;

            return true;
        }

        /// <summary>CEP válido: 8 dígitos.</summary>
        public static bool ValidateCep(string value) => OnlyDigits(value).Length == 8;
    }
}