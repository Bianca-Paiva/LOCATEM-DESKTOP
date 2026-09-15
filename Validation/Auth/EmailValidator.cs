using System.Text.RegularExpressions;

namespace LOCATEM_DESKTOP.Validation.Auth
{
    /// <summary>
    /// Mesmo regex simples usado em Login.tsx e InformeEmail.tsx: /^[^\s@]+@[^\s@]+\.[^\s@]+$/.
    /// (No Cadastro, o zod usa .email() internamente, que segue a mesma ideia — e-mail bem formado.)
    /// </summary>
    public static class EmailValidator
    {
        private static readonly Regex Regex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

        public static bool IsValid(string? email) => !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email);
    }
}
