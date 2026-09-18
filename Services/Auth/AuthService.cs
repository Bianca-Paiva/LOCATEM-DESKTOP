using System.Net.Http.Json;
using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <inheritdoc cref="IAuthService"/>
    public class AuthService : IAuthService
    {
        // Mesma base de API usada em services/authService.ts.
        private const string ApiBase = "http://localhost:5033/api";

        private readonly HttpClient _httpClient;

        public AuthService(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task CriarUsuarioAsync(CadastroPayload payload)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{ApiBase}/Cadastro/CriarUsuario",
                payload
            );

            var corpoResposta = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Status: {(int)response.StatusCode} {response.StatusCode}\n" +
                    $"Resposta da API: {corpoResposta}"
                );
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginPayload payload)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{ApiBase}/Login/login",
                payload
            );

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(erro);
            }

            var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (resultado is null)
                throw new HttpRequestException("Resposta inválida do servidor.");

            return resultado;
        }

        public async Task<UsuarioMeResponse> BuscarUsuarioLogadoAsync(string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{ApiBase}/Usuarios/me"
            );

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    token
                );

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(erro);
            }

            var usuario = await response.Content.ReadFromJsonAsync<UsuarioMeResponse>();

            if (usuario is null)
                throw new HttpRequestException(
                    "Resposta inválida ao buscar o usuário logado."
                );

            return usuario;
        }
    }
}
