using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOCATEM_DESKTOP.Models.Locacoes
{
    public class Locacao
    {
        public string Id { get; set; } = string.Empty;

        /// <summary>Liga a locação à ferramenta de origem no catálogo.</summary>
        public int ProdutoId { get; set; }

        public string Produto { get; set; } = string.Empty;
        public string Imagem { get; set; } = string.Empty;

        /// <summary>Período já formatado para exibição, ex: "15 Jul – 18 Jul 2026".</summary>
        public string Periodo { get; set; } = string.Empty;

        public string Locador { get; set; } = string.Empty;
        public string LocadorId { get; set; } = string.Empty;
        public string Locatario { get; set; } = string.Empty;
        public StatusLocacao Status { get; set; }
        public string MensagemStatus { get; set; } = string.Empty;

        /// <summary>"dd/mm/aaaa".</summary>
        public string DataInicio { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty;
        public string DataFim { get; set; } = string.Empty;
        public string HoraFim { get; set; } = string.Empty;

        public int Quantidade { get; set; }

        /// <summary>Valor total já formatado, ex: "R$ 200,00".</summary>
        public string Valor { get; set; } = string.Empty;
    }
}
