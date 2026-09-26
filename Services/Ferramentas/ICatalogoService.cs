using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Services.Ferramentas
{
    // Equivalente ao CatalogoContext/CatalogoProvider do React.
    public interface ICatalogoService
    {
        IReadOnlyList<Produto> Produtos { get; }
        event EventHandler? CatalogoAlterado;
        int? FerramentaSelecionadaId { get; set; }

        IReadOnlyList<Produto> ObterPorLocador(string? locadorId);
        void Adicionar(Produto produto);
        void Atualizar(int id, Produto produto);
        void Remover(int id);
        void SubstituirTodos(IEnumerable<Produto> produtos);
    }
}
