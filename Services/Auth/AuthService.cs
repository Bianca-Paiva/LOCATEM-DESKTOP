using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
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
                new AuthenticationHeaderValue(
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

        public async Task AtualizarPerfilAsync(
            string token,
            string nome,
            string telefone,
            string documento,
            string endereco)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"{ApiBase}/Usuarios/me"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(new
            {
                nome,
                telefone,
                documento,
                endereco
            });

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    string.IsNullOrWhiteSpace(erro)
                        ? "Não foi possível atualizar o perfil."
                        : erro);
            }
        }

        public async Task<string> UploadFotoPerfilAsync(
            string token,
            string usuarioId,
            Stream arquivo,
            string nomeArquivo,
            string? contentType = null)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{ApiBase}/Upload/foto-perfil");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(usuarioId), "UsuarioId");

            var arquivoContent = new StreamContent(arquivo);
            if (!string.IsNullOrWhiteSpace(contentType))
                arquivoContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

            form.Add(arquivoContent, "Foto", nomeArquivo);
            request.Content = form;

            var response = await _httpClient.SendAsync(request);
            var corpo = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(corpo);

            if (string.IsNullOrWhiteSpace(corpo))
                throw new HttpRequestException("A API não retornou a URL da foto enviada.");

            using var json = JsonDocument.Parse(corpo);

            if (json.RootElement.TryGetProperty("urlFoto", out var urlFoto) ||
                json.RootElement.TryGetProperty("UrlFoto", out urlFoto))
            {
                return urlFoto.GetString() ?? string.Empty;
            }

            throw new HttpRequestException("A API não retornou a URL da foto enviada.");
        }
    }
}
