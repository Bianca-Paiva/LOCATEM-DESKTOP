using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <summary>
    /// Serviço de chamadas HTTP de autenticação — migrado de services/authService.ts.
    /// Cadastro e usuários não-mockados continuam usando estes endpoints. A conta de desenvolvimento do locador é resolvida localmente no LoginViewModel antes de chegar neste serviço.
    /// </summary>
    public interface IAuthService
    {
        Task CriarUsuarioAsync(CadastroPayload payload);

        Task<LoginResponse> LoginAsync(LoginPayload payload);
        Task<UsuarioMeResponse> BuscarUsuarioLogadoAsync(string token);
    }
}
