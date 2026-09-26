using System.Collections.ObjectModel;
using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Conta;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Models.Locacoes;
using LOCATEM_DESKTOP.Models.Notificacoes;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Notificacoes;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Conta.Notificacoes
{
    /// <summary>
    /// Controla filtro, paginação, modal e ações da tela de Notificações.
    /// A estrutura reproduz o comportamento de useNotifications.ts do projeto Web.
    /// </summary>
    public sealed class NotificacoesViewModel : BaseViewModel
    {
        // Mantém a mesma quantidade de cards por página usada no Web.
        private const int TamanhoPagina = 3;

        // Rotas já disponíveis no Portal do Locador e acessíveis pelo AppHeader desta página.
        private static readonly HashSet<string> RotasMigradas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "homeLocador",
                "minhasFerramentas",
                "gerenciarLocacoes",
                "historicoLocacoes",
                "avaliacao",
                "notificacoes",
                "perfil",
                "cadastroFerramenta",
                "ferramentaDetalhe"
            };

        // Opções visíveis no filtro, mantendo a mesma ordem e nomenclatura do Web.
        public IReadOnlyList<string> OpcoesFiltro { get; } =
        [
            "Todas",
            "Hoje",
            "Ontem",
            "Esta semana",
            "Este mês"
        ];

        // Serviço de sessão usado para validar o perfil e alimentar o cabeçalho.
        private readonly IAuthSessionService _authSession;

        // Serviço compartilhado responsável por manter o estado das notificações.
        private readonly INotificacaoService _notificacaoService;

        /// <summary>
        /// Inicializa comandos e dependências usados pelos controles da página e dos componentes filhos.
        /// </summary>
        public NotificacoesViewModel(
            IAuthSessionService authSession,
            INotificacaoService notificacaoService)
        {
            // Mantém acesso à sessão do usuário autenticado.
            _authSession = authSession;

            // Utiliza a mesma fonte de notificações durante toda a execução.
            _notificacaoService = notificacaoService;

            // Navega apenas para telas que já existem no Desktop.
            NavegarCommand = new AsyncRelayCommand(async parametro =>
                await NavegarAsync(parametro as string));

            // Abre a confirmação antes de apagar todas as notificações.
            AbrirConfirmacaoLimparCommand = new RelayCommand(() =>
                ConfirmacaoLimparAberta = true,
                () => PodeLimpar);

            // Fecha a confirmação sem alterar os dados.
            CancelarLimparCommand = new RelayCommand(() =>
                ConfirmacaoLimparAberta = false);

            // Limpa a fonte completa, não somente o período filtrado.
            ConfirmarLimparCommand = new RelayCommand(LimparTudo);

            // Abre o modal de uma notificação específica.
            AbrirDetalhesCommand = new RelayCommand(parametro =>
                AbrirDetalhes(parametro as Notificacao));

            // Fecha o modal e remove a seleção atual.
            FecharDetalhesCommand = new RelayCommand(FecharDetalhes);

            // Replica o comportamento do botão Renovar do Web, removendo a notificação atendida.
            RenovarCommand = new RelayCommand(parametro =>
                Renovar(parametro as Notificacao));

            // Executa a ação contextual disponível no modal selecionado.
            AcaoModalCommand = new AsyncRelayCommand(ExecutarAcaoModalAsync);

            // Navega diretamente para uma página numérica da paginação.
            IrParaPaginaCommand = new RelayCommand(parametro =>
            {
                if (parametro is PaginaNotificacao pagina)
                    IrParaPagina(pagina.Numero);
            });

            // Retrocede uma página quando existir uma anterior.
            PaginaAnteriorCommand = new RelayCommand(
                () => IrParaPagina(PaginaAtual - 1),
                () => PaginaAnteriorDisponivel);

            // Avança uma página quando existir uma próxima.
            ProximaPaginaCommand = new RelayCommand(
                () => IrParaPagina(PaginaAtual + 1),
                () => ProximaPaginaDisponivel);
        }

        // Comando exposto ao AppHeader para a navegação principal.
        public ICommand NavegarCommand { get; }

        // Comando do botão "Limpar tudo".
        public ICommand AbrirConfirmacaoLimparCommand { get; }

        // Comando do botão "Cancelar" no modal de confirmação.
        public ICommand CancelarLimparCommand { get; }

        // Comando do botão negativo que confirma a limpeza.
        public ICommand ConfirmarLimparCommand { get; }

        // Comando acionado pelo botão "Ver detalhes" de cada card.
        public ICommand AbrirDetalhesCommand { get; }

        // Comando responsável por fechar o modal de detalhes.
        public ICommand FecharDetalhesCommand { get; }

        // Comando compartilhado pelos botões "Renovar" do card e do modal.
        public ICommand RenovarCommand { get; }

        // Comando da ação principal contextual exibida no rodapé do modal.
        public ICommand AcaoModalCommand { get; }

        // Comando dos números da paginação.
        public ICommand IrParaPaginaCommand { get; }

        // Comando da seta de página anterior.
        public ICommand PaginaAnteriorCommand { get; }

        // Comando da seta de próxima página.
        public ICommand ProximaPaginaCommand { get; }

        // Nome mostrado no avatar/cabeçalho da página.
        private string _nomeUsuario = string.Empty;
        public string NomeUsuario
        {
            get => _nomeUsuario;
            private set => SetProperty(ref _nomeUsuario, value);
        }

        // Foto do usuário autenticado quando houver uma imagem cadastrada.
        private ImageSource? _fotoUsuario;
        public ImageSource? FotoUsuario
        {
            get => _fotoUsuario;
            private set => SetProperty(ref _fotoUsuario, value);
        }

        // Texto atualmente selecionado no Picker de período.
        private string _filtroSelecionado = "Todas";
        public string FiltroSelecionado
        {
            get => _filtroSelecionado;
            set
            {
                // Ignora atribuições repetidas para evitar recalcular a lista sem necessidade.
                if (!SetProperty(ref _filtroSelecionado, value))
                    return;

                // Toda troca de filtro volta para a primeira página, como no hook React.
                PaginaAtual = 1;
                AtualizarLista();
            }
        }

        // Itens visíveis na página atual após aplicar filtro e paginação.
        private IReadOnlyList<Notificacao> _notificacoesPagina = Array.Empty<Notificacao>();
        public IReadOnlyList<Notificacao> NotificacoesPagina
        {
            get => _notificacoesPagina;
            private set => SetProperty(ref _notificacoesPagina, value);
        }

        // Indica se o filtro atual possui ao menos uma notificação.
        private bool _temNotificacoes;
        public bool TemNotificacoes
        {
            get => _temNotificacoes;
            private set
            {
                if (!SetProperty(ref _temNotificacoes, value))
                    return;

                // Atualiza propriedades derivadas usadas pelo estado vazio e pelo botão limpar.
                OnPropertyChanged(nameof(SemNotificacoes));
                OnPropertyChanged(nameof(PodeLimpar));
                AtualizarEstadoComandos();
            }
        }

        // Inverte TemNotificacoes para controlar a exibição do estado vazio.
        public bool SemNotificacoes => !TemNotificacoes;

        // Segue o Web: o botão Limpar fica desabilitado quando o período filtrado não possui itens.
        public bool PodeLimpar => TemNotificacoes;

        // Página numérica atualmente selecionada.
        private int _paginaAtual = 1;
        public int PaginaAtual
        {
            get => _paginaAtual;
            private set
            {
                if (!SetProperty(ref _paginaAtual, value))
                    return;

                // Sinaliza mudanças nas propriedades que dependem da página atual.
                OnPropertyChanged(nameof(PaginaAnteriorDisponivel));
                OnPropertyChanged(nameof(ProximaPaginaDisponivel));
            }
        }

        // Quantidade total de páginas produzida pelo filtro atual.
        private int _totalPaginas = 1;
        public int TotalPaginas
        {
            get => _totalPaginas;
            private set
            {
                if (!SetProperty(ref _totalPaginas, value))
                    return;

                // Recalcula controles dependentes do total de páginas.
                OnPropertyChanged(nameof(MostrarPaginacao));
                OnPropertyChanged(nameof(ProximaPaginaDisponivel));
            }
        }

        // A paginação numérica só aparece quando existe mais de uma página.
        public bool MostrarPaginacao => TotalPaginas > 1;

        // Controla a habilitação visual e funcional da seta esquerda.
        public bool PaginaAnteriorDisponivel => PaginaAtual > 1;

        // Controla a habilitação visual e funcional da seta direita.
        public bool ProximaPaginaDisponivel => PaginaAtual < TotalPaginas;

        // Números renderizados na paginação, incluindo o estado da página ativa.
        private IReadOnlyList<PaginaNotificacao> _paginas = Array.Empty<PaginaNotificacao>();
        public IReadOnlyList<PaginaNotificacao> Paginas
        {
            get => _paginas;
            private set => SetProperty(ref _paginas, value);
        }

        // Notificação atualmente aberta no modal; null significa modal fechado.
        private Notificacao? _notificacaoSelecionada;
        public Notificacao? NotificacaoSelecionada
        {
            get => _notificacaoSelecionada;
            private set => SetProperty(ref _notificacaoSelecionada, value);
        }

        // Controla a camada de modal de detalhes.
        private bool _modalDetalhesAberto;
        public bool ModalDetalhesAberto
        {
            get => _modalDetalhesAberto;
            private set => SetProperty(ref _modalDetalhesAberto, value);
        }

        // Linhas de dados montadas conforme a categoria da notificação selecionada.
        private IReadOnlyList<LinhaDetalheNotificacao> _linhasDetalhes = Array.Empty<LinhaDetalheNotificacao>();
        public IReadOnlyList<LinhaDetalheNotificacao> LinhasDetalhes
        {
            get => _linhasDetalhes;
            private set => SetProperty(ref _linhasDetalhes, value);
        }

        // Exibe o botão Renovar do modal apenas nas categorias equivalentes ao Web.
        private bool _mostrarRenovarModal;
        public bool MostrarRenovarModal
        {
            get => _mostrarRenovarModal;
            private set => SetProperty(ref _mostrarRenovarModal, value);
        }

        // Exibe a ação contextual somente quando existe destino implementado no Desktop.
        private bool _mostrarAcaoModal;
        public bool MostrarAcaoModal
        {
            get => _mostrarAcaoModal;
            private set => SetProperty(ref _mostrarAcaoModal, value);
        }

        // Texto do botão contextual, por exemplo "Ver locação" ou "Avaliar locação".
        private string _textoAcaoModal = string.Empty;
        public string TextoAcaoModal
        {
            get => _textoAcaoModal;
            private set => SetProperty(ref _textoAcaoModal, value);
        }

        // Controla o modal compartilhado de confirmação do botão "Limpar tudo".
        private bool _confirmacaoLimparAberta;
        public bool ConfirmacaoLimparAberta
        {
            get => _confirmacaoLimparAberta;
            private set => SetProperty(ref _confirmacaoLimparAberta, value);
        }

        /// <summary>
        /// Garante que a tela permaneça restrita ao Portal do Locador.
        /// </summary>
        public async Task<bool> GarantirAcessoAsync()
        {
            var usuario = _authSession.UsuarioAtual;

            if (usuario is not null && usuario.Tipo == TipoUsuario.Locador)
                return true;

            await Shell.Current.GoToAsync("//login");
            return false;
        }

        /// <summary>
        /// Carrega usuário e mock na primeira exibição da instância atual da página.
        /// </summary>
        /// <summary>
        /// Atualiza os dados do usuário e exibe o estado atual das notificações.
        /// </summary>
        public void Carregar()
        {
            var usuario = _authSession.UsuarioAtual;

            if (usuario is null)
                return;

            // Atualiza as informações exibidas pelo AppHeader.
            NomeUsuario = usuario.Nome;
            FotoUsuario = AvatarHelper.CriarImageSource(usuario.FotoUrl);

            // Reprocessa a lista utilizando a fonte compartilhada do serviço.
            AtualizarLista();
        }

        /// <summary>
        /// Filtra a fonte completa, corrige a página atual e monta os três cards visíveis.
        /// </summary>
        private void AtualizarLista()
        {
            // Converte o texto do Picker para o enum interno do filtro.
            var filtro = ConverterFiltro(FiltroSelecionado);

            // Usa o horário local do sistema, equivalente ao new Date() do Web.
            var agora = DateTime.Now;

            // Obtém sempre o estado atual armazenado no serviço Singleton.
            var filtradas = _notificacaoService
                .ObterTodas()
                .Where(notificacao => CorrespondeAoFiltro(notificacao, filtro, agora))
                .ToList();

            // Total de páginas nunca fica abaixo de 1 para manter os cálculos estáveis.
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(filtradas.Count / (double)TamanhoPagina));

            // Corrige a página caso uma remoção diminua a quantidade total de páginas.
            if (PaginaAtual > TotalPaginas)
                PaginaAtual = TotalPaginas;

            // Recorta somente os itens da página atual.
            NotificacoesPagina = filtradas
                .Skip((PaginaAtual - 1) * TamanhoPagina)
                .Take(TamanhoPagina)
                .ToList();

            // O estado vazio considera a lista filtrada inteira, não apenas o recorte da página.
            TemNotificacoes = filtradas.Count > 0;

            // Recria os botões numéricos para refletir imediatamente a página ativa.
            Paginas = Enumerable.Range(1, TotalPaginas)
                .Select(numero => new PaginaNotificacao
                {
                    Numero = numero,
                    Ativa = numero == PaginaAtual
                })
                .ToList();

            // Atualiza CanExecute das setas e do botão de limpeza.
            AtualizarEstadoComandos();
        }

        /// <summary>
        /// Converte a opção apresentada ao usuário no enum usado pela regra de filtro.
        /// </summary>
        private static FiltroNotificacao ConverterFiltro(string texto) => texto switch
        {
            "Hoje" => FiltroNotificacao.Hoje,
            "Ontem" => FiltroNotificacao.Ontem,
            "Esta semana" => FiltroNotificacao.EstaSemana,
            "Este mês" => FiltroNotificacao.EsteMes,
            _ => FiltroNotificacao.Todas
        };

        /// <summary>
        /// Aplica exatamente os períodos usados pelo hook de Notificações do Web.
        /// </summary>
        private static bool CorrespondeAoFiltro(
            Notificacao notificacao,
            FiltroNotificacao filtro,
            DateTime agora)
        {
            return filtro switch
            {
                FiltroNotificacao.Hoje => notificacao.Data.Date == agora.Date,
                FiltroNotificacao.Ontem => notificacao.Data.Date == agora.Date.AddDays(-1),
                FiltroNotificacao.EstaSemana =>
                    notificacao.Data >= InicioDaSemana(agora) && notificacao.Data <= agora,
                FiltroNotificacao.EsteMes =>
                    notificacao.Data.Year == agora.Year && notificacao.Data.Month == agora.Month,
                _ => true
            };
        }

        /// <summary>
        /// Retorna a segunda-feira da semana informada com o horário zerado.
        /// </summary>
        private static DateTime InicioDaSemana(DateTime data)
        {
            // Sunday precisa voltar seis dias; os demais dias voltam até segunda-feira.
            var diferenca = data.DayOfWeek == DayOfWeek.Sunday
                ? -6
                : (int)DayOfWeek.Monday - (int)data.DayOfWeek;

            return data.Date.AddDays(diferenca);
        }

        /// <summary>
        /// Abre uma página válida e recalcula os cards exibidos.
        /// </summary>
        private void IrParaPagina(int pagina)
        {
            if (pagina < 1 || pagina > TotalPaginas || pagina == PaginaAtual)
                return;

            PaginaAtual = pagina;
            AtualizarLista();
        }

        /// <summary>
        /// Remove todas as notificações da fonte compartilhada
        /// e atualiza imediatamente a interface.
        /// </summary>
        private void LimparTudo()
        {
            // Remove os registros do serviço Singleton.
            _notificacaoService.LimparTodas();

            // Fecha a confirmação após concluir a operação.
            ConfirmacaoLimparAberta = false;

            // Retorna a paginação para o início.
            PaginaAtual = 1;

            // Garante que nenhum modal permaneça associado a um item removido.
            FecharDetalhes();

            // Atualiza cards, paginação e estado vazio.
            AtualizarLista();
        }

        /// <summary>
        /// Remove a notificação atendida pelo botão Renovar e mantém a paginação consistente.
        /// </summary>
        private void Renovar(Notificacao? notificacao)
        {
            if (notificacao is null)
                return;

            // Remove a notificação atendida da fonte compartilhada.
            _notificacaoService.Remover(notificacao.Id);

            if (NotificacaoSelecionada?.Id == notificacao.Id)
                FecharDetalhes();

            AtualizarLista();
        }

        /// <summary>
        /// Seleciona uma notificação e prepara as informações específicas do modal.
        /// </summary>
        private void AbrirDetalhes(Notificacao? notificacao)
        {
            if (notificacao is null)
                return;

            NotificacaoSelecionada = notificacao;
            LinhasDetalhes = MontarLinhasDetalhes(notificacao);
            MostrarRenovarModal =
                notificacao.MostrarRenovar &&
                notificacao.Categoria is CategoriaNotificacao.DevolucaoPendente or CategoriaNotificacao.DevolucaoAtrasada;

            // Configura somente ações cujo destino já existe no Desktop.
            var acao = ObterAcaoDisponivel(notificacao);
            TextoAcaoModal = acao.Texto;
            MostrarAcaoModal = acao.Mostrar;
            ModalDetalhesAberto = true;
        }

        /// <summary>
        /// Fecha o modal e limpa os dados transitórios da seleção.
        /// </summary>
        private void FecharDetalhes()
        {
            ModalDetalhesAberto = false;
            NotificacaoSelecionada = null;
            LinhasDetalhes = Array.Empty<LinhaDetalheNotificacao>();
            MostrarRenovarModal = false;
            MostrarAcaoModal = false;
            TextoAcaoModal = string.Empty;
        }

        /// <summary>
        /// Monta somente as linhas previstas para a categoria selecionada no Web.
        /// </summary>
        private static IReadOnlyList<LinhaDetalheNotificacao> MontarLinhasDetalhes(Notificacao notificacao)
        {
            var detalhes = notificacao.Detalhes;

            // Helper local reduz repetição e mantém o fallback visual "-" do modal React.
            static LinhaDetalheNotificacao Linha(string rotulo, string? valor) => new()
            {
                Rotulo = rotulo,
                Valor = string.IsNullOrWhiteSpace(valor) ? "-" : valor
            };

            return notificacao.Categoria switch
            {
                CategoriaNotificacao.LocacaoConfirmada =>
                [
                    Linha("Equipamento", detalhes.Equipamento),
                    Linha("Status", detalhes.Status),
                    Linha("Confirmado em", detalhes.DataConfirmacao),
                    Linha("Período da locação", detalhes.PeriodoLocacao),
                    Linha("Valor", detalhes.Valor),
                    Linha("Forma de pagamento", detalhes.FormaPagamento)
                ],

                CategoriaNotificacao.DevolucaoPendente =>
                [
                    Linha("Equipamento", detalhes.Equipamento),
                    Linha("Status", detalhes.Status),
                    Linha("Data limite", detalhes.DataLimite)
                ],

                CategoriaNotificacao.EntregaAndamento =>
                [
                    Linha("Equipamento", detalhes.Equipamento),
                    Linha("Status da entrega", detalhes.StatusEntrega),
                    Linha("Previsão de chegada", detalhes.PrevisaoChegada)
                ],

                CategoriaNotificacao.FerramentaDevolvida =>
                [
                    Linha("Equipamento", detalhes.Equipamento),
                    Linha("Status", detalhes.Status),
                    Linha("Devolvido em", detalhes.DataDevolucao)
                ],

                CategoriaNotificacao.PagamentoPendente =>
                [
                    Linha("Equipamento", detalhes.Equipamento),
                    Linha("Status do pagamento", detalhes.StatusPagamento),
                    Linha("Valor", detalhes.Valor)
                ],

                CategoriaNotificacao.LocacaoCancelada =>
                [
                    Linha("Equipamento", detalhes.Equipamento),
                    Linha("Motivo", detalhes.MotivoCancelamento),
                    Linha("Cancelado em", detalhes.DataCancelamento),
                    Linha("Valor reembolsado", detalhes.ValorReembolso)
                ],

                CategoriaNotificacao.DevolucaoAtrasada =>
                [
                    Linha("Equipamento", detalhes.Equipamento),
                    Linha("Data limite", detalhes.DataLimite),
                    Linha("Dias em atraso", detalhes.DiasAtraso),
                    Linha("Multa", detalhes.Multa)
                ],

                CategoriaNotificacao.EntregaConcluida =>
                [
                    Linha("Equipamento", detalhes.Equipamento),
                    Linha("Entregue em", detalhes.DataEntrega),
                    Linha("Recebido por", detalhes.RecebidoPor)
                ],

                CategoriaNotificacao.PagamentoConfirmado =>
                [
                    Linha("Valor", detalhes.Valor),
                    Linha("Forma de pagamento", detalhes.FormaPagamento),
                    Linha("Confirmado em", detalhes.DataConfirmacao)
                ],

                CategoriaNotificacao.PagamentoRecusado =>
                [
                    Linha("Valor", detalhes.Valor),
                    Linha("Forma de pagamento", detalhes.FormaPagamento),
                    Linha("Motivo da recusa", detalhes.MotivoRecusa)
                ],

                CategoriaNotificacao.PromocaoDisponivel =>
                [
                    Linha("Categoria", detalhes.CategoriaEquipamento),
                    Linha("Cupom", detalhes.Cupom),
                    Linha("Desconto", detalhes.Desconto),
                    Linha("Válido até", detalhes.Validade)
                ],

                CategoriaNotificacao.AvaliacaoPendente =>
                [
                    Linha("Equipamento", detalhes.Equipamento),
                    Linha("Devolvido em", detalhes.DataDevolucao),
                    Linha("Nota sugerida", detalhes.NotaSugerida)
                ],

                CategoriaNotificacao.NovaMensagem =>
                [
                    Linha("De", detalhes.Remetente),
                    Linha("Assunto", detalhes.Assunto),
                    Linha("Mensagem", detalhes.Mensagem)
                ],

                _ => Array.Empty<LinhaDetalheNotificacao>()
            };
        }

        /// <summary>
        /// Define o texto da ação do modal apenas quando existe tela equivalente já migrada.
        /// </summary>
        private static (string Texto, bool Mostrar) ObterAcaoDisponivel(Notificacao notificacao)
        {
            // Avaliações possuem rota própria e mantêm a ação original do Web.
            if (notificacao.Categoria == CategoriaNotificacao.AvaliacaoPendente &&
                !string.IsNullOrWhiteSpace(notificacao.LocacaoId))
            {
                return ("Avaliar locação", true);
            }

            // Promoção e mensagem não têm destino equivalente no Portal Desktop atual.
            if (notificacao.Categoria is CategoriaNotificacao.PromocaoDisponivel or CategoriaNotificacao.NovaMensagem)
                return (string.Empty, false);

            // Demais categorias ligadas a uma locação podem abrir a área operacional ou o histórico.
            if (!string.IsNullOrWhiteSpace(notificacao.LocacaoId))
            {
                return notificacao.Categoria switch
                {
                    CategoriaNotificacao.PagamentoPendente => ("Efetuar pagamento", true),
                    CategoriaNotificacao.PagamentoRecusado => ("Tentar pagamento novamente", true),
                    CategoriaNotificacao.LocacaoCancelada => ("Ver detalhes", true),
                    _ => ("Ver locação", true)
                };
            }

            return (string.Empty, false);
        }

        /// <summary>
        /// Executa a ação contextual usando apenas rotas existentes no Desktop.
        /// </summary>
        private async Task ExecutarAcaoModalAsync()
        {
            var notificacao = NotificacaoSelecionada;
            if (notificacao is null)
                return;

            // Fecha primeiro para impedir interação duplicada durante a navegação.
            FecharDetalhes();

            // Avaliação pendente direciona para a tela de avaliações já migrada.
            if (notificacao.Categoria == CategoriaNotificacao.AvaliacaoPendente)
            {
                await Shell.Current.GoToAsync("avaliacao");
                return;
            }

            // Estados encerrados são visualizados no Histórico; os demais ficam em Gerenciar Locações.
            var encerrada = notificacao.StatusLocacao is
                StatusLocacao.Finalizada or
                StatusLocacao.Recusada or
                StatusLocacao.Cancelada;

            await Shell.Current.GoToAsync(encerrada ? "historicoLocacoes" : "gerenciarLocacoes");
        }

        /// <summary>
        /// Atualiza CanExecute dos comandos que dependem do estado da paginação ou do filtro.
        /// </summary>
        private void AtualizarEstadoComandos()
        {
            (AbrirConfirmacaoLimparCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (PaginaAnteriorCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ProximaPaginaCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Navegação do AppHeader mantendo o mesmo padrão das outras páginas internas.
        /// </summary>
        private static async Task NavegarAsync(string? rota)
        {
            if (string.IsNullOrWhiteSpace(rota) || !RotasMigradas.Contains(rota))
                return;

            // Evita empilhar novamente a própria rota atual.
            if (Shell.Current.CurrentState.Location.OriginalString.EndsWith(
                    rota,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Home é rota raiz do Shell e precisa de navegação absoluta.
            if (string.Equals(rota, "homeLocador", StringComparison.OrdinalIgnoreCase))
                await Shell.Current.GoToAsync("//homeLocador");
            else
                await Shell.Current.GoToAsync(rota);
        }
    }
}
