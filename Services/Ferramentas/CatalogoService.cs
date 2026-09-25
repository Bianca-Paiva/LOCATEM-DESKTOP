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

        public void Adicionar(Produto produto)
        {
            produto.Id = _produtos.Count == 0 ? 1 : _produtos.Max(p => p.Id) + 1;
            _produtos.Add(produto);
            NotificarAlteracao();
        }

        public void Atualizar(int id, Produto produto)
        {
            var indice = _produtos.FindIndex(p => p.Id == id);
            if (indice < 0) throw new InvalidOperationException("Ferramenta não encontrada.");
            produto.Id = id;
            _produtos[indice] = produto;
            NotificarAlteracao();
        }

        /// Notifica as telas abertas — usado pelas operações de escrita do catálogo.
        protected void NotificarAlteracao() => CatalogoAlterado?.Invoke(this, EventArgs.Empty);
    }
}
