using LOCATEM_DESKTOP.Models.Locacoes;
using LOCATEM_DESKTOP.Models.Notificacoes;

namespace LOCATEM_DESKTOP.Services.Notificacoes
{
    /// <summary>
    /// Fonte temporária da tela de Notificações.
    /// Os registros reproduzem o mock utilizado atualmente no projeto Web.
    /// </summary>
    public static class NotificacoesMock
    {
        /// <summary>
        /// Cria uma nova lista a cada carregamento para evitar compartilhar alterações entre instâncias.
        /// </summary>
        public static List<Notificacao> Criar() =>
        [
            new()
            {
                Id = "1",
                Tipo = TipoNotificacao.Sucesso,
                Categoria = CategoriaNotificacao.LocacaoConfirmada,
                Titulo = "Locação Confirmada",
                Descricao = "Sua locação da Pistola de Pintura The Black Tools foi confirmada.",
                Timestamp = "04/07/2026 às 10h15",
                Data = new DateTime(2026, 7, 4, 10, 15, 0),
                StatusLocacao = StatusLocacao.PreparandoEntrega,
                LocacaoId = "3",
                Detalhes = new DetalhesNotificacao
                {
                    Equipamento = "Pistola de Pintura The Black Tools",
                    Status = "Confirmada",
                    DataConfirmacao = "04/07/2026 às 10h15",
                    PeriodoLocacao = "05/07/2026 a 08/07/2026 (3 dias)",
                    Valor = "R$ 89,90",
                    FormaPagamento = "Cartão de crédito •••• 4521"
                }
            },
            new()
            {
                Id = "2",
                Tipo = TipoNotificacao.Aviso,
                Categoria = CategoriaNotificacao.DevolucaoPendente,
                Titulo = "Devolução Pendente",
                Descricao = "A devolução do Aparador De Grama Bipartido Tramontina deve ser feita amanhã.",
                Timestamp = "03/07/2026 às 17h00",
                Data = new DateTime(2026, 7, 3, 17, 0, 0),
                MostrarRenovar = true,
                StatusLocacao = StatusLocacao.AguardandoDevolucao,
                LocacaoId = "6",
                Detalhes = new DetalhesNotificacao
                {
                    Equipamento = "Aparador De Grama Bipartido Tramontina",
                    Status = "Pendente",
                    DataLimite = "05/07/2026 às 18h00"
                }
            },
            new()
            {
                Id = "3",
                Tipo = TipoNotificacao.Entrega,
                Categoria = CategoriaNotificacao.EntregaAndamento,
                Titulo = "Entrega em Andamento",
                Descricao = "A Serra Circular Profissional DESOON 24 Dentes está a caminho.",
                Timestamp = string.Empty,
                Data = new DateTime(2026, 7, 4, 13, 0, 0),
                InformacaoExtra = "Tempo estimado de chegada: Hoje às 15:00",
                StatusLocacao = StatusLocacao.EmTransporte,
                LocacaoId = "4",
                Detalhes = new DetalhesNotificacao
                {
                    Equipamento = "Serra Circular Profissional DESOON 24 Dentes",
                    StatusEntrega = "Saiu para entrega",
                    PrevisaoChegada = "Hoje às 15:00"
                }
            },
            new()
            {
                Id = "4",
                Tipo = TipoNotificacao.Sucesso,
                Categoria = CategoriaNotificacao.FerramentaDevolvida,
                Titulo = "Ferramenta Devolvida",
                Descricao = "A devolução da Parafusadeira Furadeira de Impacto Hanabi foi registrada.",
                Timestamp = "01/07/2026 às 09h40",
                Data = new DateTime(2026, 7, 1, 9, 40, 0),
                StatusLocacao = StatusLocacao.Finalizada,
                LocacaoId = "10",
                Detalhes = new DetalhesNotificacao
                {
                    Equipamento = "Parafusadeira Furadeira de Impacto Hanabi",
                    Status = "Devolvida sem avarias",
                    DataDevolucao = "01/07/2026 às 09h40"
                }
            },
            new()
            {
                Id = "5",
                Tipo = TipoNotificacao.Aviso,
                Categoria = CategoriaNotificacao.PagamentoPendente,
                Titulo = "Pagamento Pendente",
                Descricao = "O pagamento da locação da Parafusadeira e Furadeira WAP 12V ainda não foi confirmado.",
                Timestamp = "20/06/2026 às 14h20",
                Data = new DateTime(2026, 6, 20, 14, 20, 0),
                MostrarRenovar = true,
                StatusLocacao = StatusLocacao.AguardandoPagamento,
                LocacaoId = "2",
                Detalhes = new DetalhesNotificacao
                {
                    Equipamento = "Parafusadeira e Furadeira WAP 12V",
                    StatusPagamento = "Aguardando confirmação",
                    Valor = "R$ 145,00"
                }
            },
            new()
            {
                Id = "6",
                Tipo = TipoNotificacao.Erro,
                Categoria = CategoriaNotificacao.LocacaoCancelada,
                Titulo = "Locação Cancelada",
                Descricao = "Sua locação da Furadeira Parafusadeira Sem Fio A Bateria Tb-12e 12v 3/8 10mm Com Maleta E Acessórios The Black Tools foi cancelada.",
                Timestamp = "19/06/2026 às 11h05",
                Data = new DateTime(2026, 6, 19, 11, 5, 0),
                StatusLocacao = StatusLocacao.Cancelada,
                LocacaoId = "9",
                Detalhes = new DetalhesNotificacao
                {
                    Equipamento = "Furadeira Parafusadeira Sem Fio A Bateria Tb-12e 12v 3/8 10mm Com Maleta E Acessórios The Black Tools",
                    MotivoCancelamento = "Equipamento indisponível na data solicitada",
                    DataCancelamento = "19/06/2026 às 11h05",
                    ValorReembolso = "R$ 65,00"
                }
            },
            new()
            {
                Id = "7",
                Tipo = TipoNotificacao.Erro,
                Categoria = CategoriaNotificacao.DevolucaoAtrasada,
                Titulo = "Devolução em Atraso",
                Descricao = "A devolução da Betoneira está atrasada.",
                Timestamp = "18/06/2026 às 09h00",
                Data = new DateTime(2026, 6, 18, 9, 0, 0),
                MostrarRenovar = true,
                LocacaoId = "6",
                Detalhes = new DetalhesNotificacao
                {
                    Equipamento = "Betoneira 400L CSM",
                    DataLimite = "17/06/2026 às 18h00",
                    DiasAtraso = "1 dia",
                    Multa = "R$ 15,00"
                }
            },
            new()
            {
                Id = "8",
                Tipo = TipoNotificacao.Sucesso,
                Categoria = CategoriaNotificacao.EntregaConcluida,
                Titulo = "Entrega Concluída",
                Descricao = "A Serra Circular Makita foi entregue com sucesso.",
                Timestamp = "17/06/2026 às 15h20",
                Data = new DateTime(2026, 6, 17, 15, 20, 0),
                StatusLocacao = StatusLocacao.EmAndamento,
                LocacaoId = "5",
                Detalhes = new DetalhesNotificacao
                {
                    Equipamento = "Serra Circular Makita 5007NB",
                    DataEntrega = "17/06/2026 às 15h20",
                    RecebidoPor = "Portaria do condomínio"
                }
            },
            new()
            {
                Id = "9",
                Tipo = TipoNotificacao.Sucesso,
                Categoria = CategoriaNotificacao.PagamentoConfirmado,
                Titulo = "Pagamento Confirmado",
                Descricao = "O pagamento da locação da Lixadeira Bosch foi aprovado.",
                Timestamp = "16/06/2026 às 10h30",
                Data = new DateTime(2026, 6, 16, 10, 30, 0),
                StatusLocacao = StatusLocacao.PreparandoEntrega,
                LocacaoId = "3",
                Detalhes = new DetalhesNotificacao
                {
                    Valor = "R$ 89,90",
                    FormaPagamento = "Cartão de crédito •••• 4521",
                    DataConfirmacao = "16/06/2026 às 10h30"
                }
            },
            new()
            {
                Id = "10",
                Tipo = TipoNotificacao.Erro,
                Categoria = CategoriaNotificacao.PagamentoRecusado,
                Titulo = "Pagamento Recusado",
                Descricao = "Não conseguimos aprovar o pagamento da sua locação.",
                Timestamp = "15/06/2026 às 08h45",
                Data = new DateTime(2026, 6, 15, 8, 45, 0),
                StatusLocacao = StatusLocacao.AguardandoPagamento,
                LocacaoId = "2",
                Detalhes = new DetalhesNotificacao
                {
                    Valor = "R$ 145,00",
                    FormaPagamento = "Cartão de crédito •••• 1187",
                    MotivoRecusa = "Limite insuficiente"
                }
            },
            new()
            {
                Id = "11",
                Tipo = TipoNotificacao.Promocao,
                Categoria = CategoriaNotificacao.PromocaoDisponivel,
                Titulo = "Oferta Especial",
                Descricao = "20% de desconto em ferramentas elétricas essa semana.",
                Timestamp = "14/06/2026 às 09h00",
                Data = new DateTime(2026, 6, 14, 9, 0, 0),
                Detalhes = new DetalhesNotificacao
                {
                    CategoriaEquipamento = "Ferramentas elétricas",
                    Cupom = "LOCATEM20",
                    Desconto = "20%",
                    Validade = "21/06/2026"
                }
            },
            new()
            {
                Id = "12",
                Tipo = TipoNotificacao.Lembrete,
                Categoria = CategoriaNotificacao.AvaliacaoPendente,
                Titulo = "Avalie sua locação",
                Descricao = "Conte pra gente como foi alugar a Parafusadeira Dewalt.",
                Timestamp = "13/06/2026 às 12h00",
                Data = new DateTime(2026, 6, 13, 12, 0, 0),
                StatusLocacao = StatusLocacao.Finalizada,
                LocacaoId = "10",
                Detalhes = new DetalhesNotificacao
                {
                    Equipamento = "Parafusadeira Dewalt DCF680N",
                    DataDevolucao = "01/07/2026 às 09h40",
                    NotaSugerida = "5 estrelas"
                }
            },
            new()
            {
                Id = "13",
                Tipo = TipoNotificacao.Mensagem,
                Categoria = CategoriaNotificacao.NovaMensagem,
                Titulo = "Nova Mensagem",
                Descricao = "Você recebeu uma mensagem do suporte Locatem.",
                Timestamp = "12/06/2026 às 17h10",
                Data = new DateTime(2026, 6, 12, 17, 10, 0),
                Detalhes = new DetalhesNotificacao
                {
                    Remetente = "Suporte Locatem",
                    Assunto = "Sobre sua locação",
                    Mensagem = "Olá! Confirmamos que seu equipamento já está separado para devolução."
                }
            }
        ];
    }
}
