using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Helpers.Avaliacao
{
    // Média e quantidade calculadas das avaliações reais. Migrado de utils/Avaliacao/avaliacoesResumo.ts.
    public record ResumoAvaliacoes(double Media, int Quantidade);

    public static class AvaliacoesResumo
    {
        public static ResumoAvaliacoes Calcular(IReadOnlyCollection<AvaliacaoProduto>? avaliacoes)
        {
            var lista = avaliacoes ?? Array.Empty<AvaliacaoProduto>();
            if (lista.Count == 0) return new ResumoAvaliacoes(0, 0);

            // Arredonda para 1 casa decimal (mesmo formato exibido hoje, ex: "4.7").
            var media = Math.Round(lista.Sum(a => (double)a.Nota) / lista.Count, 1);
            return new ResumoAvaliacoes(media, lista.Count);
        }
    }
}
