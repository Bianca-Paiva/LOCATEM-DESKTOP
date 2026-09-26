using LOCATEM_DESKTOP.Models.Notificacoes;

namespace LOCATEM_DESKTOP.Services.Notificacoes
{
    /// <summary>
    /// Mantém as notificações em memória enquanto o aplicativo estiver aberto.
    /// Como o serviço é Singleton, todas as telas acessam a mesma lista.
    /// </summary>
    public sealed class NotificacaoService : INotificacaoService
    {
        // O mock é carregado apenas quando a instância Singleton é criada.
        private readonly List<Notificacao> _notificacoes;

        /// <summary>
        /// Inicializa a fonte compartilhada utilizando os dados mockados.
        /// </summary>
        public NotificacaoService()
        {
            _notificacoes = NotificacoesMock.Criar();
        }

        /// <summary>
        /// Retorna o estado atual das notificações sem recriar o mock.
        /// </summary>
        public IReadOnlyList<Notificacao> ObterTodas()
        {
            return _notificacoes;
        }

        /// <summary>
        /// Remove todas as notificações da lista compartilhada.
        /// </summary>
        public void LimparTodas()
        {
            _notificacoes.Clear();
        }

        /// <summary>
        /// Remove somente a notificação correspondente ao identificador informado.
        /// </summary>
        public void Remover(string id)
        {
            _notificacoes.RemoveAll(notificacao =>
                notificacao.Id == id);
        }
    }
}