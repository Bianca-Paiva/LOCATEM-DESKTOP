using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <summary>
    /// Usuário local de desenvolvimento usado para permitir o login do locador
    /// sem depender da API.
    /// </summary>
    public static class UsuariosMock
    {
        public const string EmailLocadorTeste = "joao.silva@exemplo.com";
        public const string SenhaTeste = "Senha@123";

        /// <summary>
        /// O mock contém somente um usuário e somente o perfil Locador.
        /// </summary>
        public static readonly List<Usuario> Usuarios = new()
        {
            new Usuario
            {
                Id = "u-locador-1",
                Nome = "João da Silva",
                Email = EmailLocadorTeste,
                Telefone = "(11) 98765-4321",
                Documento = "12.345.678/0001-90",
                Endereco = "Rua das Acácias, 247 – Apto 32, São Paulo, SP · 01310-100",
                Tipo = TipoUsuario.Locador,
                LocadorId = "loc-jb",
                EmailVerificado = false,
                Desde = 2026,
                Reputacao = new ReputacaoUsuario
                {
                    Rating = 4.5,
                    TotalAvaliacoes = 145,
                    LocacoesConcluidas = 212,
                    EntregasNoPrazoPercentual = 98
                }
            }
        };

        /// <summary>
        /// Informa se o e-mail digitado pertence à conta local do locador.
        /// </summary>
        public static bool EhUsuarioDeTeste(string email) =>
            !string.IsNullOrWhiteSpace(email) &&
            string.Equals(
                email.Trim(),
                EmailLocadorTeste,
                StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Autentica o locador localmente, sem realizar qualquer chamada à API.
        /// </summary>
        public static Usuario? AutenticarUsuarioTeste(string email, string senha)
        {
            if (!EhUsuarioDeTeste(email) ||
                !string.Equals(senha, SenhaTeste, StringComparison.Ordinal))
            {
                return null;
            }

            var usuario = Usuarios[0];

            // Retorna uma cópia para que alterações na sessão não modifiquem o mock estático.
            return new Usuario
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Token = $"mock-token-{usuario.Id}",
                Telefone = usuario.Telefone,
                Documento = usuario.Documento,
                Endereco = usuario.Endereco,
                Tipo = usuario.Tipo,
                LocadorId = usuario.LocadorId,
                FotoUrl = usuario.FotoUrl,
                EmailVerificado = usuario.EmailVerificado,
                Desde = usuario.Desde,
                Reputacao = new ReputacaoUsuario
                {
                    Rating = usuario.Reputacao.Rating,
                    TotalAvaliacoes = usuario.Reputacao.TotalAvaliacoes,
                    LocacoesConcluidas = usuario.Reputacao.LocacoesConcluidas,
                    EntregasNoPrazoPercentual = usuario.Reputacao.EntregasNoPrazoPercentual
                }
            };
        }

        public static Usuario? BuscarPorEmail(string email) =>
            EhUsuarioDeTeste(email) ? Usuarios[0] : null;
    }
}
