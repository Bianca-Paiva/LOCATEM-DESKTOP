using System.Text.RegularExpressions;

namespace LOCATEM_DESKTOP.Validation.Auth
{
    public class PasswordValidationItem
    {
        public string Label { get; set; } = string.Empty;
        public bool Valid { get; set; }
    }

    public enum PasswordFormResultType
    {
        Required,
        Mismatch,
        Fraca,
        Media,
        Success
    }

    /// <summary>
    /// Migrado de validation/Password/passwordStrength.ts e validation/Password/passwordValidation.ts.
    /// Centraliza toda a lógica de força de senha e confirmação, usada tanto em CadastroPage
    /// quanto em InformeNovaSenhaPage.
    /// </summary>
    public static class PasswordValidator
    {
        public static PasswordStrengthResult CheckPasswordStrength(string? value)
        {
            value ??= string.Empty;

            var requirements = new PasswordRequirements
            {
                Tamanho = value.Length >= 8,
                Minuscula = Regex.IsMatch(value, "[a-z]"),
                Maiuscula = Regex.IsMatch(value, "[A-Z]"),
                Numero = Regex.IsMatch(value, "[0-9]"),
                Especial = Regex.IsMatch(value, @"[^A-Za-z0-9]")
            };

            var score = new[] { requirements.Tamanho, requirements.Minuscula, requirements.Maiuscula, requirements.Numero, requirements.Especial }
                .Count(v => v);

            var strength = PasswordStrength.Nenhuma;
            if (value.Length > 0)
            {
                strength = score <= 2 ? PasswordStrength.Fraca
                         : score <= 4 ? PasswordStrength.Media
                         : PasswordStrength.Forte;
            }

            return new PasswordStrengthResult
            {
                Requirements = requirements,
                Strength = strength,
                IsStrong = score == 5
            };
        }

        public static List<PasswordValidationItem> GetPasswordValidations(string? password)
        {
            password ??= string.Empty;
            return new List<PasswordValidationItem>
            {
                new() { Label = "Pelo menos 8 caracteres", Valid = password.Length >= 8 },
                new() { Label = "Pelo menos uma letra maiúscula", Valid = Regex.IsMatch(password, "[A-Z]") },
                new() { Label = "Pelo menos uma letra minúscula", Valid = Regex.IsMatch(password, "[a-z]") },
                new() { Label = "Pelo menos um número", Valid = Regex.IsMatch(password, @"\d") },
                new() { Label = "Pelo menos um caractere especial", Valid = Regex.IsMatch(password, "[!@#$%^&*(),.?\":{}|<>]") },
            };
        }

        /// <summary>"erro" | "sucesso" | "" do getConfirmPasswordStatus original — aqui como string? nula.</summary>
        public static bool? GetConfirmPasswordStatus(string? password, string? confirmPassword)
        {
            if (string.IsNullOrEmpty(confirmPassword)) return null;
            return password == confirmPassword;
        }

        public static string GetConfirmPasswordError(string? password, string? confirmPassword)
        {
            if (string.IsNullOrEmpty(confirmPassword)) return string.Empty;
            return password != confirmPassword ? "As senhas não coincidem" : string.Empty;
        }

        public static PasswordFormResultType ValidatePasswordForm(string? password, string? confirmPassword, PasswordStrengthResult strength)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                return PasswordFormResultType.Required;

            if (password != confirmPassword)
                return PasswordFormResultType.Mismatch;

            if (!strength.IsStrong)
                return strength.Strength == PasswordStrength.Media ? PasswordFormResultType.Media : PasswordFormResultType.Fraca;

            return PasswordFormResultType.Success;
        }
    }
}
