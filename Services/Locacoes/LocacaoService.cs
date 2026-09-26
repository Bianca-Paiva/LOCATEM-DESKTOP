using LOCATEM_DESKTOP.Models.Locacoes;
using LOCATEM_DESKTOP.Services.Ferramentas;

namespace LOCATEM_DESKTOP.Services.Locacoes
{
    public class LocacaoService : ILocacaoService
    {
        private const string MensagemCancelamentoAutomatico =
            "Locação cancelada automaticamente por falta de pagamento dentro do prazo.";

        private readonly List<Locacao> _locacoes;

        public LocacaoService(ICatalogoService catalogo)
        {
            _locacoes = LocacoesMock.Criar(catalogo.Produtos);
        }

        public IReadOnlyList<Locacao> Locacoes => _locacoes;

        public event EventHandler? LocacoesAlteradas;

        public IReadOnlyList<Locacao> ObterPorLocador(string? locadorId)
        {
            if (string.IsNullOrWhiteSpace(locadorId))
                return Array.Empty<Locacao>();

            return _locacoes
                .Where(l => string.Equals(l.LocadorId, locadorId, StringComparison.Ordinal))
                .ToList();
        }

        public bool AtualizarLocacao(string id, Action<Locacao> atualizar)
        {
            if (string.IsNullOrWhiteSpace(id) || atualizar is null)
                return false;

            var locacao = _locacoes.FirstOrDefault(l => l.Id == id);
            if (locacao is null)
                return false;

            atualizar(locacao);
            NotificarAlteracao();
            return true;
        }

        public bool CancelarPagamentosVencidos()
        {
            var agora = DateTimeOffset.Now;
            var houveAlteracao = false;

            foreach (var locacao in _locacoes.Where(l =>
                         l.Status == StatusLocacao.AguardandoPagamento &&
                         l.PrazoPagamento.HasValue &&
                         agora > l.PrazoPagamento.Value))
            {
                locacao.Status = StatusLocacao.Cancelada;
                locacao.MensagemStatus = MensagemCancelamentoAutomatico;
                locacao.MotivoCancelamento = MensagemCancelamentoAutomatico;
                houveAlteracao = true;
            }

            if (houveAlteracao)
                NotificarAlteracao();

            return houveAlteracao;
        }

        private void NotificarAlteracao() =>
            LocacoesAlteradas?.Invoke(this, EventArgs.Empty);
    }
}
