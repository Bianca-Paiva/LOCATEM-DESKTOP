using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOCATEM_DESKTOP.Models.Ferramentas
{
    public class Produto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;

        // Valor da diária já no formato exibido, ex: "25,00"
        public string Price { get; set; } = string.Empty;

        // Nomes das imagens em Resources/Images/ProdutosImg.
        public List<string> Images { get; set; } = new();

        // Nome da loja/anunciante, ex: "JB Ferramentas".
        public string Locador { get; set; } = string.Empty;

        // Identificador do locador dono do anúncio — usado para filtrar as telas do locador.
        public string LocadorId { get; set; } = string.Empty;

        public string Localizacao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public StatusFerramenta Status { get; set; }

        //Data de cadastro do anúncio, formato "dd/mm/aaaa".
        public string? CadastradoEm { get; set; }

        public List<AvaliacaoProduto> Avaliacoes { get; set; } = new();
    }
}
