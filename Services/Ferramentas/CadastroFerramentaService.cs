using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Services.Ferramentas;

public interface ICadastroFerramentaService
{
    Task CadastrarAsync(string token, CadastroFerramentaDados dados, IReadOnlyList<FotoFerramentaDados> fotos);
    Task EditarAsync(string token, int ferramentaId, CadastroFerramentaDados dados);
    Task<IReadOnlyList<Produto>> ObterMinhasAsync(string token);
}

public record CadastroFerramentaDados(
    string Nome,
    string Marca,
    string Modelo,
    string Descricao,
    IReadOnlyList<string> Acessorios,
    decimal Diaria,
    decimal Caucao,
    int CategoriaId,
    int QuantidadeDisponivel,
    string EstadoConservacao,
    string FonteAlimentacao,
    IReadOnlyList<EspecificacaoFerramenta> EspecificacoesTecnicas,
    IReadOnlyList<string> DiasIndisponiveis,
    string TipoAprovacao,
    EnderecoRetiradaDados EnderecoRetirada);

public record EnderecoRetiradaDados(
    string CEP,
    string Logradouro,
    string Numero,
    string Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    int TipoEndereco = 1,
    bool EhPrioritario = false);

public record FotoFerramentaDados(string NomeArquivo, string ContentType, byte[] Bytes);

// Integração do Desktop com os endpoints reais de Ferramenta.
public class CadastroFerramentaService : ICadastroFerramentaService
{
    private const string ApiBase = "http://localhost:5033/api";
    private const string ApiOrigin = "http://localhost:5033";

    private readonly HttpClient _http = new();

    public async Task CadastrarAsync(
        string token,
        CadastroFerramentaDados dados,
        IReadOnlyList<FotoFerramentaDados> fotos)
    {
        using var request = CriarRequisicao(HttpMethod.Post, $"{ApiBase}/Ferramenta", token);
        request.Content = JsonContent.Create(CriarPayload(dados));

        using var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(body)
                    ? "Erro ao cadastrar ferramenta."
                    : body);

        if (fotos.Count == 0)
            return;

        using var json = JsonDocument.Parse(body);
        if (!json.RootElement.TryGetProperty("ferramentaId", out var id) &&
            !json.RootElement.TryGetProperty("FerramentaId", out id))
        {
            throw new HttpRequestException(
                "A API não retornou o identificador da ferramenta para enviar as fotos.");
        }

        using var upload = CriarRequisicao(
            HttpMethod.Post,
            $"{ApiBase}/Upload/foto-ferramenta",
            token);

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
        {
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(uploadBody)
                    ? "Erro ao enviar fotos da ferramenta."
                    : uploadBody);
        }
    }

    public async Task EditarAsync(
        string token,
        int ferramentaId,
        CadastroFerramentaDados dados)
    {
        using var request = CriarRequisicao(
            HttpMethod.Put,
            $"{ApiBase}/Ferramenta/{ferramentaId}",
            token);

        request.Content = JsonContent.Create(CriarPayload(dados));

        using var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(body)
                    ? "Erro ao editar ferramenta."
                    : body);
        }
    }

    public async Task<IReadOnlyList<Produto>> ObterMinhasAsync(string token)
    {
        using var request = CriarRequisicao(
            HttpMethod.Get,
            $"{ApiBase}/Ferramenta/Minhas",
            token);

        using var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(body)
                    ? "Não foi possível carregar suas ferramentas."
                    : body);
        }

        var itens = JsonSerializer.Deserialize<List<FerramentaApiResponse>>(
            body,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<FerramentaApiResponse>();

        return itens.Select(MapearProduto).ToList();
    }

    private static object CriarPayload(CadastroFerramentaDados dados)
    {
        return new
        {
            nome = dados.Nome,
            marca = dados.Marca,
            modelo = dados.Modelo,
            descricao = dados.Descricao,
            acessorios = dados.Acessorios,
            diaria = dados.Diaria,
            caucao = dados.Caucao,
            categoriaId = dados.CategoriaId,
            quantidadeDisponivel = dados.QuantidadeDisponivel,
            estadoConservacao = dados.EstadoConservacao,
            fonteAlimentacao = dados.FonteAlimentacao,
            especificacoesTecnicas = dados.EspecificacoesTecnicas,
            diasIndisponiveis = dados.DiasIndisponiveis,
            tipoAprovacao = dados.TipoAprovacao,
            enderecoRetirada = new
            {
                cep = dados.EnderecoRetirada.CEP,
                logradouro = dados.EnderecoRetirada.Logradouro,
                numero = dados.EnderecoRetirada.Numero,
                complemento = dados.EnderecoRetirada.Complemento,
                bairro = dados.EnderecoRetirada.Bairro,
                cidade = dados.EnderecoRetirada.Cidade,
                estado = dados.EnderecoRetirada.Estado,
                tipoEndereco = dados.EnderecoRetirada.TipoEndereco,
                ehPrioritario = dados.EnderecoRetirada.EhPrioritario
            }
        };
    }

    private static HttpRequestMessage CriarRequisicao(
        HttpMethod metodo,
        string url,
        string token)
    {
        var request = new HttpRequestMessage(metodo, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static Produto MapearProduto(FerramentaApiResponse item)
    {
        var status = item.Disponibilidade switch
        {
            2 => StatusFerramenta.Locada,
            4 => StatusFerramenta.Manutencao,
            _ => item.Status == 1
                ? StatusFerramenta.Disponivel
                : StatusFerramenta.Indisponivel
        };

        return new Produto
        {
            Id = item.FerramentaId,
            Title = item.Nome,
            Marca = item.Marca,
            Modelo = item.Modelo,
            Price = item.Diaria.ToString("0.00", CultureInfo.InvariantCulture),
            Images = item.Fotos
                .OrderBy(f => f.Id)
                .Select(f => NormalizarUrlImagem(f.UrlImagem))
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .ToList(),
            Locador = item.UsuarioNome,
            LocadorId = item.UsuarioId.ToString(CultureInfo.InvariantCulture),
            Localizacao = item.Localizacao,
            Categoria = item.CategoriaNome,
            Status = status,
            EstadoConservacao = item.EstadoConservacao,
            QuantidadeDisponivel = item.QuantidadeDisponivel,
            FonteAlimentacao = item.FonteAlimentacao,
            Descricao = item.Descricao,
            Especificacoes = item.EspecificacoesTecnicas
                .Select(e => new EspecificacaoFerramenta
                {
                    Label = e.Label,
                    Valor = e.Valor
                })
                .ToList(),
            Caucao = item.Caucao.ToString("0.00", CultureInfo.InvariantCulture),
            Acessorios = item.Acessorios.ToList(),
            DiasIndisponiveis = item.DiasIndisponiveis.ToList(),
            TipoAprovacao = item.TipoAprovacao,
            Cep = item.EnderecoRetirada?.CEP ?? string.Empty,
            RuaAvenida = item.EnderecoRetirada?.Logradouro ?? string.Empty,
            Numero = item.EnderecoRetirada?.Numero ?? string.Empty,
            Complemento = item.EnderecoRetirada?.Complemento ?? string.Empty,
            Bairro = item.EnderecoRetirada?.Bairro ?? string.Empty,
            Cidade = item.EnderecoRetirada?.Cidade ?? string.Empty,
            Estado = item.EnderecoRetirada?.Estado ?? string.Empty,
            CadastradoEm = item.DataCadastro.ToLocalTime().ToString("dd/MM/yyyy"),
            AvaliacaoMedia = item.AvaliacaoMedia,
            TotalAvaliacoes = item.TotalAvaliacoes
        };
    }

    private static string NormalizarUrlImagem(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        var valor = url.Trim();
        if (valor.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            valor.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return valor;
        }

        return $"{ApiOrigin}/{valor.TrimStart('/')}";
    }

    private sealed class FerramentaApiResponse
    {
        public int FerramentaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public List<string> Acessorios { get; set; } = new();
        public decimal Diaria { get; set; }
        public decimal Caucao { get; set; }
        public DateTime DataCadastro { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Disponibilidade { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public string EstadoConservacao { get; set; } = string.Empty;
        public string FonteAlimentacao { get; set; } = string.Empty;
        public List<EspecificacaoApi> EspecificacoesTecnicas { get; set; } = new();
        public List<string> DiasIndisponiveis { get; set; } = new();
        public string TipoAprovacao { get; set; } = "manual";
        public EnderecoApi? EnderecoRetirada { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public List<FotoApi> Fotos { get; set; } = new();
        public double AvaliacaoMedia { get; set; }
        public int TotalAvaliacoes { get; set; }
    }

    private sealed class EspecificacaoApi
    {
        public string Label { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
    }

    private sealed class EnderecoApi
    {
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
    }

    private sealed class FotoApi
    {
        public int Id { get; set; }
        public string UrlImagem { get; set; } = string.Empty;
    }
}
