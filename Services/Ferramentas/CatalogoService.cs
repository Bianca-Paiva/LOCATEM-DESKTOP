using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Services.Ferramentas
{
    public class CatalogoService : ICatalogoService
    {
        private readonly List<Produto> _produtos = ProdutosMock.Criar();

        public IReadOnlyList<Produto> Produtos => _produtos;

        public event EventHandler? CatalogoAlterado;

        public int? FerramentaSelecionadaId { get; set; }

        public IReadOnlyList<Produto> ObterPorLocador(string? locadorId)
        {
            if (string.IsNullOrWhiteSpace(locadorId)) return Array.Empty<Produto>();

            return _produtos
                .Where(p => !string.IsNullOrEmpty(p.LocadorId) && p.LocadorId == locadorId)
                .ToList();
        }

        /// Notifica as telas abertas — usado pelas operações de escrita do catálogo.
        protected void NotificarAlteracao() => CatalogoAlterado?.Invoke(this, EventArgs.Empty);
    }
}
