using LOCATEM_DESKTOP.Models.Notificacoes;

namespace LOCATEM_DESKTOP.Services.Notificacoes
{
    /// <summary>
    /// Define as operações utilizadas para consultar e alterar
    /// o estado das notificações durante a execução do aplicativo.
    /// </summary>
    public interface INotificacaoService
    {
        /// <summary>
        /// Retorna todas as notificações disponíveis atualmente.
        /// </summary>
        IReadOnlyList<Notificacao> ObterTodas();

        /// <summary>
        /// Remove todas as notificações da fonte compartilhada.
        /// </summary>
        void LimparTodas();

        /// <summary>
        /// Remove uma notificação específica pelo identificador.
        /// </summary>
        void Remover(string id);
    }
}