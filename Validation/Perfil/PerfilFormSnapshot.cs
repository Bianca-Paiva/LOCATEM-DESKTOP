using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Validation.Perfil
{
    /// <summary>Campos editáveis do modal de Perfil, equivalente a PerfilFormData do React.</summary>
    public class PerfilFormSnapshot
    {
        public TipoUsuario Tipo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
    }

    /// <summary>Erros por campo, equivalente ao formState.errors do react-hook-form.</summary>
    public class PerfilFieldErrors
    {
        public string? Nome { get; set; }
        public string? Telefone { get; set; }
        public string? Documento { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }

        public bool HasErrors =>
            Nome is not null || Telefone is not null || Documento is not null ||
            Cep is not null || Logradouro is not null || Numero is not null;
    }
}
