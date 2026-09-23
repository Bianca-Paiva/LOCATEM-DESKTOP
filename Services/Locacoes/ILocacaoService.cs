using LOCATEM_DESKTOP.Models.Locacoes;

namespace LOCATEM_DESKTOP.Services.Locacoes
{
    //
    // Equivalente ao LocacaoContext/LocacaoProvider do React: fonte única de verdade das locações
    // na sessão atual. Registrado como singleton no DI (MauiProgram.cs).
    // 
    public interface ILocacaoService
    {
        IReadOnlyList<Locacao> Locacoes { get; }

        // Disparado quando a lista de locações muda.
        event EventHandler? LocacoesAlteradas;

        // Locações de um locador específico (sempre pelo identificador, nunca pelo nome).
        IReadOnlyList<Locacao> ObterPorLocador(string? locadorId);
    }
}
