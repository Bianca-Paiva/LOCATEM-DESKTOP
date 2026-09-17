using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOCATEM_DESKTOP.Models.Locacoes
{
    public enum StatusLocacao
    {
        Pendente,
        AguardandoPagamento,
        Confirmada,
        PreparandoEntrega,
        EmTransporte,
        EmAndamento,
        AguardandoDevolucao,
        DevolucaoEmTransporte,
        Finalizada,
        Recusada,
        Cancelada
    }
}
