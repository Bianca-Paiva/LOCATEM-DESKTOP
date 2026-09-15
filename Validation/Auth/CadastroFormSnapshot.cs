using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Validation.Auth
{
    /// <summary>
    /// Snapshot dos campos do formulário de Cadastro no momento da validação/submissão.
    /// Equivalente ao "CadastroFormData" inferido do zod schema (cadastroSchema.ts).
    /// </summary>
    public class CadastroFormSnapshot
    {
        public TipoUsuario Tipo { get; set; } = TipoUsuario.Locatario;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string ConfirmarSenha { get; set; } = string.Empty;
    }

    /// <summary>Mensagens de erro por campo — equivalente ao "errors" do react-hook-form.</summary>
    public class CadastroFieldErrors
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Documento { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Senha { get; set; }
        public string? ConfirmarSenha { get; set; }

        public bool HasErrors =>
            Nome != null || Email != null || Telefone != null || Documento != null || Cep != null ||
            Logradouro != null || Numero != null || Senha != null || ConfirmarSenha != null;
    }
}
