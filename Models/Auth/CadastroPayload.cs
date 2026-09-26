namespace LOCATEM_DESKTOP.Models.Auth
{
    /// <summary>Payload enviado para POST /Cadastro/CriarUsuario. Migrado de services/authService.ts.</summary>
    public class CadastroPayload
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string ConfirmarSenha { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;

        // Endereço completo enviado para o mesmo DTO utilizado pelo backend.
        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        /// <summary>1 = locatário, 2 = locador (mesma convenção do React).</summary>
        public int TipoUsuario { get; set; }
    }
}
