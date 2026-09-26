using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LOCATEM_DESKTOP.Services.Ferramentas;

public interface ICadastroFerramentaService
{
    Task CadastrarAsync(string token, CadastroFerramentaDados dados, IReadOnlyList<FotoFerramentaDados> fotos);
}

public record CadastroFerramentaDados(
    string Nome, string Marca, string Modelo, string Descricao,
    IReadOnlyList<string> Acessorios, decimal Diaria, decimal Caucao, int CategoriaId);

public record FotoFerramentaDados(string NomeArquivo, string ContentType, byte[] Bytes);

// Os endpoints e o payload correspondem a services/ferramentaservice.ts.
public class CadastroFerramentaService : ICadastroFerramentaService
{
    private const string ApiBase = "http://localhost:5033/api";
    private readonly HttpClient _http = new();

    public async Task CadastrarAsync(string token, CadastroFerramentaDados dados, IReadOnlyList<FotoFerramentaDados> fotos)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiBase}/Ferramenta");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new
        {
            nome = dados.Nome, marca = dados.Marca, modelo = dados.Modelo,
            descricao = dados.Descricao, acessorios = dados.Acessorios,
            diaria = dados.Diaria, caucao = dados.Caucao, categoriaId = dados.CategoriaId
        });

        using var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(string.IsNullOrWhiteSpace(body) ? "Erro ao cadastrar ferramenta." : body);

        if (fotos.Count == 0) return;
        using var json = JsonDocument.Parse(body);
        if (!json.RootElement.TryGetProperty("ferramentaId", out var id) &&
            !json.RootElement.TryGetProperty("FerramentaId", out id))
            throw new HttpRequestException("A API não retornou o identificador da ferramenta para enviar as fotos.");

        using var upload = new HttpRequestMessage(HttpMethod.Post, $"{ApiBase}/Upload/foto-ferramenta");
        upload.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(id.ToString()), "FerramentaId");
        foreach (var foto in fotos)
        {
            var content = new ByteArrayContent(foto.Bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);
            form.Add(content, "Fotos", foto.NomeArquivo);
        }
        upload.Content = form;
        using var uploadResponse = await _http.SendAsync(upload);
        var uploadBody = await uploadResponse.Content.ReadAsStringAsync();
        if (!uploadResponse.IsSuccessStatusCode)
            throw new HttpRequestException(string.IsNullOrWhiteSpace(uploadBody) ? "Erro ao enviar fotos da ferramenta." : uploadBody);
    }
}
