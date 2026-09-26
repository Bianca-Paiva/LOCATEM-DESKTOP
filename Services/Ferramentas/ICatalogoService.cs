using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Services.Ferramentas
{
    //
    // Equivalente ao CatalogoContext/CatalogoProvider do React: fonte única de verdade do catálogo
    // de ferramentas na sessão atual. Registrado como singleton no DI (MauiProgram.cs), assim como
    // o provider React fica acessível via useCatalogoStore().
    //
    public interface ICatalogoService
    {
        IReadOnlyList<Produto> Produtos { get; }

        // Disparado quando o catálogo muda (cadastro, edição, remoção de ferramenta).
        event EventHandler? CatalogoAlterado;


        // Id da ferramenta escolhida em "Minhas Ferramentas"/Home (via "Ver"/"Editar") — lido pelas
        // telas de Detalhe e Cadastro da Ferramenta. Null = cadastro de uma ferramenta nova.

        int? FerramentaSelecionadaId { get; set; }

        // Ferramentas de um locador específico (sempre pelo identificador, nunca pelo nome).
        IReadOnlyList<Produto> ObterPorLocador(string? locadorId);
        void Adicionar(Produto produto);
        void Atualizar(int id, Produto produto);
        void Remover(int id);
    }
}
