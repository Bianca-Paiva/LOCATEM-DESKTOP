using System.Net.Http.Json;
using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <inheritdoc cref="IAuthService"/>
    public class AuthService : IAuthService
    {
        // Mesma base de API usada em services/authService.ts.
        private const string ApiBase = "https://localhost:7127/api";

        private readonly HttpClient _httpClient;

        public AuthService(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task CriarUsuarioAsync(CadastroPayload payload)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiBase}/Cadastro/CriarUsuario", payload);
            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(erro);
            }
        }

        public async Task<string> LoginAsync(LoginPayload payload)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiBase}/Login", payload);
            var conteudo = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(conteudo);

            return conteudo;
        }
    }
}
