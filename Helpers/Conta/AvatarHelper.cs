namespace LOCATEM_DESKTOP.Helpers.Conta
{
    /// <summary>
    /// Regra única para avatares sem foto: primeira letra do primeiro e do último nome.
    /// Equivalente ao utilitário de iniciais usado pelo Avatar do React.
    /// </summary>
    public static class AvatarHelper
    {
        public static string ExtrairIniciais(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return "?";

            var partes = nome
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length == 1)
                return partes[0][..1].ToUpperInvariant();

            return $"{partes[0][0]}{partes[^1][0]}".ToUpperInvariant();
        }

        /// <summary>
        /// Converte a URL/caminho persistido do perfil em ImageSource. Também resolve caminhos
        /// relativos devolvidos pela API (ex.: /uploads/foto.jpg).
        /// </summary>
        public static ImageSource? CriarImageSource(string? valor, string apiBase = "http://localhost:5033")
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            if (Uri.TryCreate(valor, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return ImageSource.FromUri(uri);
            }

            if (valor.StartsWith('/'))
                return ImageSource.FromUri(new Uri($"{apiBase.TrimEnd('/')}{valor}"));

            if (File.Exists(valor))
                return ImageSource.FromFile(valor);

            return null;
        }
    }
}
