using LOCATEM_DESKTOP.Models.Locacoes;
using LOCATEM_DESKTOP.Services.Ferramentas;

namespace LOCATEM_DESKTOP.Services.Locacoes
{
    
    public class LocacaoService : ILocacaoService
    {
        private readonly List<Locacao> _locacoes;

        public LocacaoService(ICatalogoService catalogo)
        {
            // Cada locação deriva de um produto real do catálogo, como no mock do React.
            _locacoes = LocacoesMock.Criar(catalogo.Produtos);
        }

        public IReadOnlyList<Locacao> Locacoes => _locacoes;

        public event EventHandler? LocacoesAlteradas;

        public IReadOnlyList<Locacao> ObterPorLocador(string? locadorId)
        {
            if (string.IsNullOrWhiteSpace(locadorId)) return Array.Empty<Locacao>();

            return _locacoes.Where(l => l.LocadorId == locadorId).ToList();
        }

        //Notifica as telas abertas — usado pelas operações de escrita das locações.
        protected void NotificarAlteracao() => LocacoesAlteradas?.Invoke(this, EventArgs.Empty);
    }
}
