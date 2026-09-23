namespace LOCATEM_DESKTOP.Models.Auth
{
    /// <summary>
    /// Usuário autenticado. Nesta versão do desktop, exclusiva para locadores, o usuário é
    /// sempre tratado como Locador.
    /// Migrado de types/Auth/usuario.types.ts.
    ///
    /// O projeto ainda não tem uma API real de autenticação (ver Services/Auth/AuthService.cs,
    /// que possui os endpoints prontos porém não chamados no fluxo de Login), então esses dados
    /// são preenchidos a partir do catálogo mockado em Services/Auth/UsuariosMock.cs.
    /// </summary>
    public class Usuario
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string Telefone { get; set; } = string.Empty;

        /// <summary>CNPJ (locador) — sem máscara ou com, conforme preenchido no cadastro/edição.</summary>
        public string Documento { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public TipoUsuario Tipo { get; set; }

        /// <summary>Somente para Tipo == Locador: identificador da loja/locador.</summary>
        public string? LocadorId { get; set; }

        /// <summary>Ausente = avatar cai para as iniciais do nome.</summary>
        public string? FotoUrl { get; set; }

        public bool EmailVerificado { get; set; }

        /// <summary>Ano de criação da conta — exibido como "Locador desde {ano}".</summary>
        public int Desde { get; set; }

        public ReputacaoUsuario Reputacao { get; set; } = new();

        /// <summary>
        /// Cria uma cópia com os campos informados sobrescritos (equivalente ao spread
        /// "{ ...atual, ...dados }" usado em AuthProvider.atualizarUsuario).
        /// </summary>
        public Usuario CopiarCom(
            string? nome = null,
            string? email = null,

            string? telefone = null,
            string? documento = null,
            string? endereco = null,
            string? fotoUrl = null,
            bool? emailVerificado = null)
        {
            return new Usuario
            {
                Id = Id,
                Nome = nome ?? Nome,
                Email = email ?? Email,
                Token = Token,
                Telefone = telefone ?? Telefone,
                Documento = documento ?? Documento,
                Endereco = endereco ?? Endereco,
                Tipo = Tipo,
                LocadorId = LocadorId,
                FotoUrl = fotoUrl ?? FotoUrl,
                EmailVerificado = emailVerificado ?? EmailVerificado,
                Desde = Desde,
                Reputacao = Reputacao
            };
        }
    }
}