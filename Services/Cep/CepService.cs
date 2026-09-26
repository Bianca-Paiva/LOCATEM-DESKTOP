using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LOCATEM_DESKTOP.Models.Cep;

namespace LOCATEM_DESKTOP.Services.Cep
{
    /// <summary>Serviço reutilizável de consulta de CEP via ViaCEP.</summary>
    public class CepService : ICepService
    {
        private readonly HttpClient _httpClient = new();

        public async Task<CepResultado?> ConsultarAsync(
            string cep,
            CancellationToken cancellationToken = default)
        {
            var cepLimpo = Regex.Replace(cep ?? string.Empty, @"\D", string.Empty);

            if (cepLimpo.Length != 8)
                return null;

            var resposta = await _httpClient.GetFromJsonAsync<ViaCepResponse>(
                $"https://viacep.com.br/ws/{cepLimpo}/json/",
                cancellationToken);

            if (resposta is null || resposta.Erro)
                return null;

            return new CepResultado
            {
                Cep = resposta.Cep ?? string.Empty,
                Logradouro = resposta.Logradouro ?? string.Empty,
                Bairro = resposta.Bairro ?? string.Empty,
                Cidade = resposta.Localidade ?? string.Empty,
                Estado = resposta.Uf ?? string.Empty
            };
        }

        private sealed class ViaCepResponse
        {
            [JsonPropertyName("cep")]
            public string? Cep { get; set; }

            [JsonPropertyName("logradouro")]
            public string? Logradouro { get; set; }

            [JsonPropertyName("bairro")]
            public string? Bairro { get; set; }

            [JsonPropertyName("localidade")]
            public string? Localidade { get; set; }

            [JsonPropertyName("uf")]
            public string? Uf { get; set; }

            [JsonPropertyName("erro")]
            public bool Erro { get; set; }
        }
    }
}
