namespace LOCATEM_DESKTOP.Models.Auth
{
    /// <summary>Payload enviado para POST /Login. Migrado de services/authService.ts.</summary>
    public class LoginPayload
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
