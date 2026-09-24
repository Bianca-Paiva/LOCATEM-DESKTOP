using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <summary>
    /// Serviço HTTP de autenticação e dados do usuário. Mantém os mesmos endpoints usados pelo
    /// services/authService.ts do projeto Web para que Perfil e autenticação compartilhem a API.
    /// </summary>
    public interface IAuthService
    {
        Task CriarUsuarioAsync(CadastroPayload payload);

        Task<LoginResponse> LoginAsync(LoginPayload payload);

        Task<UsuarioMeResponse> BuscarUsuarioLogadoAsync(string token);

        Task AtualizarPerfilAsync(
            string token,
            string nome,
            string telefone,
            string documento,
            string endereco);

        Task<string> UploadFotoPerfilAsync(
            string token,
            string usuarioId,
            Stream arquivo,
            string nomeArquivo,
            string? contentType = null);
    }
}
