using LOCATEM_DESKTOP.Models.Locacoes;

namespace LOCATEM_DESKTOP.Services.Locacoes
{
    /// <summary>
    /// Fonte única de verdade das locações na sessão atual, equivalente ao
    /// LocacaoContext/LocacaoProvider do projeto Web.
    /// </summary>
    public interface ILocacaoService
    {
        IReadOnlyList<Locacao> Locacoes { get; }

        event EventHandler? LocacoesAlteradas;

        IReadOnlyList<Locacao> ObterPorLocador(string? locadorId);

        /// <summary>Atualiza uma locação existente e notifica as telas interessadas.</summary>
        bool AtualizarLocacao(string id, Action<Locacao> atualizar);

        /// <summary>Cancela locações cujo prazo de pagamento expirou.</summary>
        bool CancelarPagamentosVencidos();
    }
}
