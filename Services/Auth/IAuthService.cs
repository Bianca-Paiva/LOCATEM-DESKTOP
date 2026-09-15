using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <summary>
    /// Serviço de chamadas HTTP de autenticação — migrado de services/authService.ts.
    /// Usado hoje apenas pelo fluxo de Cadastro (CriarUsuarioAsync), assim como no React o
    /// Cadastro já chama criarUsuario() de verdade enquanto o Login ainda usa o mock local
    /// (ver IAuthSessionService).
    /// </summary>
    public interface IAuthService
    {
        Task CriarUsuarioAsync(CadastroPayload payload);

        Task<string> LoginAsync(LoginPayload payload);
    }
}
