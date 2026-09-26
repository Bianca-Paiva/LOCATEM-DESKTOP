using LOCATEM_DESKTOP.Components.Locacoes;
using LOCATEM_DESKTOP.Models.Notificacoes;
using MauiIcons.Material;

namespace LOCATEM_DESKTOP.Helpers.Notificacoes
{
    /// <summary>
    /// Configuração visual utilizada pelos cards e pelo modal de Notificações.
    /// Status de locação reaproveitam a configuração já centralizada no Desktop.
    /// </summary>
    public sealed record NotificacaoVisual(
        MaterialIcons Icone,
        string CorIcone,
        string CorFundo);

    /// <summary>
    /// Resolve ícone e cores de uma notificação sem duplicar a regra visual em cada componente.
    /// </summary>
    public static class NotificacaoVisualHelper
    {
        /// <summary>
        /// Prioriza a aparência do status da locação quando a notificação estiver vinculada a um status.
        /// </summary>
        public static NotificacaoVisual Obter(Notificacao notificacao)
        {
            if (notificacao.StatusLocacao is not null)
            {
                var status = StatusLocacaoConfig.Obter(notificacao.StatusLocacao.Value);
                return new NotificacaoVisual(status.Icone, status.Cor, status.Fundo);
            }

            return notificacao.Tipo switch
            {
                TipoNotificacao.Sucesso => new(MaterialIcons.CheckCircle, "#2EAE60", "#DCF5E3"),
                TipoNotificacao.Aviso => new(MaterialIcons.Schedule, "#E8A33D", "#FDECC8"),
                TipoNotificacao.Entrega => new(MaterialIcons.LocalShipping, "#3B82F6", "#DCEBFC"),
                TipoNotificacao.Erro => new(MaterialIcons.Close, "#E34747", "#FBDCDC"),
                TipoNotificacao.Informacao => new(MaterialIcons.Assignment, "#475569", "#E2E8F0"),
                TipoNotificacao.Promocao => new(MaterialIcons.Star, "#A855F7", "#F2DCFC"),
                TipoNotificacao.Mensagem => new(MaterialIcons.Person, "#6366F1", "#DEE2FC"),
                TipoNotificacao.Lembrete => new(MaterialIcons.Notifications, "#D97706", "#FDE7C8"),
                _ => new(MaterialIcons.Notifications, "#475569", "#E2E8F0")
            };
        }
    }
}
