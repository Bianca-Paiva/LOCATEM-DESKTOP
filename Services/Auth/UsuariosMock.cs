using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <summary>
    /// Catálogo mockado de usuários — migrado de mocks/usuarios.mock.ts. Fonte única de verdade
    /// enquanto não existe uma API de autenticação real conectada (o endpoint de login em
    /// AuthService já está pronto, mas o fluxo de Login usa este mock, exatamente como no React).
    ///
    /// Cobre os mesmos cenários do original:
    /// - Locador sem foto, com dados quase completos (perfil incompleto)
    /// - Locatário com foto e e-mail verificado (perfil 100% completo)
    /// - Qualquer outro e-mail cai no fallback (usuário novo, perfil bem incompleto)
    /// </summary>
    public static class UsuariosMock
    {
        public static readonly List<Usuario> Usuarios = new()
        {
            new Usuario
            {
                Id = "u-locador-1",
                Nome = "João da Silva",
                Email = "joao.silva@exemplo.com",
                Telefone = "(11) 98765-4321",
                Documento = "12.345.678/0001-90",
                Endereco = "Rua das Acácias, 247 – Apto 32, São Paulo, SP · 01310-100",
                Tipo = TipoUsuario.Locador,
                LocadorId = "loc-jb",
                EmailVerificado = false,
                Desde = 2026,
                Reputacao = new ReputacaoUsuario { Rating = 4.5, TotalAvaliacoes = 145, LocacoesConcluidas = 212, EntregasNoPrazoPercentual = 98 }
            },
            new Usuario
            {
                Id = "u-locador-2",
                Nome = "Marcos Andrade",
                Email = "marcos.andrade@exemplo.com",
                Telefone = "(11) 97654-3210",
                Documento = "23.456.789/0001-11",
                Endereco = "Rua Voluntários da Pátria, 980, São Paulo, SP · 02011-000",
                Tipo = TipoUsuario.Locador,
                LocadorId = "loc-ms",
                EmailVerificado = true,
                Desde = 2025,
                Reputacao = new ReputacaoUsuario { Rating = 4.0, TotalAvaliacoes = 20, LocacoesConcluidas = 96, EntregasNoPrazoPercentual = 94 }
            },
            new Usuario
            {
                Id = "u-locador-3",
                Nome = "Wagner Zanetti",
                Email = "wagner.zanetti@exemplo.com",
                Telefone = "(11) 96543-2109",
                Documento = "34.567.890/0001-22",
                Endereco = "Av. Radial Leste, 3200, São Paulo, SP · 03102-000",
                Tipo = TipoUsuario.Locador,
                LocadorId = "loc-wz",
                EmailVerificado = true,
                Desde = 2025,
                Reputacao = new ReputacaoUsuario { Rating = 4.3, TotalAvaliacoes = 96, LocacoesConcluidas = 138, EntregasNoPrazoPercentual = 97 }
            },
            new Usuario
            {
                Id = "u-locataria-1",
                Nome = "Maria Oliveira",
                Email = "maria.oliveira@exemplo.com",
                Telefone = "(11) 91234-5678",
                Documento = "987.654.321-00",
                Endereco = "Av. Sapopemba, 1500, São Paulo, SP · 03988-000",
                Tipo = TipoUsuario.Locatario,
                FotoUrl = "https://i.pravatar.cc/150?u=maria.oliveira",
                EmailVerificado = true,
                Desde = 2026,
                Reputacao = new ReputacaoUsuario { Rating = 4.8, TotalAvaliacoes = 38, LocacoesConcluidas = 20 }
            }
        };

        /// <summary>Busca um usuário mockado pelo e-mail digitado no login.</summary>
        public static Usuario? BuscarPorEmail(string email) =>
            Usuarios.FirstOrDefault(u => string.Equals(u.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Fallback para e-mails que não estão no catálogo mockado: simula um usuário
        /// recém-cadastrado, com poucos dados preenchidos (perfil bem incompleto).
        /// </summary>
        public static Usuario CriarFallback(string email)
        {
            var nomeBase = email.Split('@')[0].Replace(".", " ").Replace("_", " ").Trim();
            if (string.IsNullOrWhiteSpace(nomeBase)) nomeBase = "Usuário";

            var nomeFormatado = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nomeBase);

            return new Usuario
            {
                Id = $"u-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                Nome = nomeFormatado,
                Email = email,
                Telefone = string.Empty,
                Documento = string.Empty,
                Endereco = string.Empty,
                Tipo = TipoUsuario.Locatario,
                EmailVerificado = false,
                Desde = DateTime.UtcNow.Year,
                Reputacao = new ReputacaoUsuario { Rating = 0, TotalAvaliacoes = 0, LocacoesConcluidas = 0 }
            };
        }
    }
}
