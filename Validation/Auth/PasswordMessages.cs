namespace LOCATEM_DESKTOP.Validation.Auth
{
    /// <summary>Migrado de validation/Password/passwordMessages.ts.</summary>
    public static class PasswordMessages
    {
        public static readonly AlertaMessage Required = new("Campos obrigatórios", "Por favor, preencha os dois campos de senha para continuar.");
        public static readonly AlertaMessage Mismatch = new("Erro", "As senhas não coincidem.");
        public static readonly AlertaMessage Weak = new("Senha fraca", "A senha escolhida possui um nível fraco de segurança. Adicione todos os requisitos para torná-la forte.");
        public static readonly AlertaMessage Medium = new("Senha média", "Sua senha possui um nível médio de segurança. Adicione todos os requisitos para torná-la forte.");
    }
}
