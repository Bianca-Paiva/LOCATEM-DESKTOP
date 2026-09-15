namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <inheritdoc cref="IRedirectAposLoginService"/>
    public class RedirectAposLoginService : IRedirectAposLoginService
    {
        // Mesma lista de ROTAS_VALIDAS do React — ver nota de escopo na interface.
        private static readonly HashSet<string> RotasValidas = new(StringComparer.OrdinalIgnoreCase)
        {
            "carrinho",
            "produtoDetalhe"
        };

        private string? _rotaPendente;

        public void MarcarRedirect(string rota)
        {
            if (RotasValidas.Contains(rota))
                _rotaPendente = rota;
        }

        public string? LerRedirect() => _rotaPendente is not null && RotasValidas.Contains(_rotaPendente) ? _rotaPendente : null;

        public void LimparRedirect() => _rotaPendente = null;
    }
}
