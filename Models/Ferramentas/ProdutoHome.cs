using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LOCATEM_DESKTOP.Helpers.Avaliacao;

namespace LOCATEM_DESKTOP.Models.Ferramentas
{
    public class ProdutoHome
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Locador { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;

        // Primeira imagem do anúncio (a única exibida no card).
        public string Imagem { get; set; } = string.Empty;

        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public StatusFerramenta Status { get; set; }

        // Nota exibida no card, ex: "4,7".
        public string RatingTexto => Rating.ToString("0.0");

        public static ProdutoHome DeProduto(Produto produto)
        {
            // Média e quantidade sempre calculadas das avaliações reais, nunca de campos fixos.
            var resumo = AvaliacoesResumo.Calcular(produto.Avaliacoes);

            return new ProdutoHome
            {
                Id = produto.Id,
                Title = produto.Title,
                Locador = produto.Locador,
                Price = produto.Price,
                Imagem = produto.Images.FirstOrDefault() ?? string.Empty,
                Rating = resumo.Media,
                ReviewCount = resumo.Quantidade,
                Status = produto.Status
            };
        }
    }
}
