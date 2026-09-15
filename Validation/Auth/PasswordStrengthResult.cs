namespace LOCATEM_DESKTOP.Validation.Auth
{
    /// <summary>Migrado de validation/Password/passwordStrength.ts (PasswordRequirements).</summary>
    public class PasswordRequirements
    {
        public bool Tamanho { get; set; }
        public bool Minuscula { get; set; }
        public bool Maiuscula { get; set; }
        public bool Numero { get; set; }
        public bool Especial { get; set; }
    }

    /// <summary>Migrado de validation/Password/passwordStrength.ts (PasswordStrength union type).</summary>
    public enum PasswordStrength
    {
        Nenhuma,
        Fraca,
        Media,
        Forte
    }

    /// <summary>Migrado de validation/Password/passwordStrength.ts (PasswordStrengthResult).</summary>
    public class PasswordStrengthResult
    {
        public PasswordRequirements Requirements { get; set; } = new();
        public PasswordStrength Strength { get; set; }
        public bool IsStrong { get; set; }
    }
}
