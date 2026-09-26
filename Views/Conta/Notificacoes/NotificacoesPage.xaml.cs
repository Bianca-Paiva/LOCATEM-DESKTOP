using LOCATEM_DESKTOP.ViewModels.Conta.Notificacoes;

namespace LOCATEM_DESKTOP.Views.Conta.Notificacoes
{
    /// <summary>
    /// Página Desktop de Notificações.
    /// O code-behind cuida apenas do ciclo de vida e da responsividade; regras permanecem no ViewModel.
    /// </summary>
    public partial class NotificacoesPage : ContentPage
    {
        // Mesmo limite amplo do Web para não restringir desnecessariamente monitores grandes.
        private const double LarguraMaximaConteudo = 2000;

        // Referência ao ViewModel injetado pelo container de dependências.
        private readonly NotificacoesViewModel _viewModel;

        // Cache evita reaplicar configurações de largura quando a faixa não mudou.
        private bool? _conteudoLimitado;
        private int _ultimaFaixaLargura = -1;

        /// <summary>
        /// Inicializa bindings e registra a atualização responsiva da janela.
        /// </summary>
        public NotificacoesPage(NotificacoesViewModel viewModel)
        {
            InitializeComponent();

            // Define a única fonte de dados da página e de seus componentes filhos.
            _viewModel = viewModel;
            BindingContext = viewModel;

            // Recalcula padding e largura conforme o usuário redimensiona a janela Desktop.
            SizeChanged += (_, _) => AtualizarResponsividade();
        }

        /// <summary>
        /// Valida o acesso e carrega as notificações sempre que a página se torna visível.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Carrega somente quando a sessão pertence ao Portal do Locador.
            if (await _viewModel.GarantirAcessoAsync())
                _viewModel.Carregar();

            // Garante que a primeira renderização já respeite a largura atual da janela.
            AtualizarResponsividade();
        }

        /// <summary>
        /// Replica os três níveis de espaçamento do CSS responsivo da página Web.
        /// </summary>
        private void AtualizarResponsividade()
        {
            if (Width <= 0)
                return;

            // Centraliza apenas em telas maiores que o limite definido pelo layout Web.
            var limitar = Width > LarguraMaximaConteudo;
            if (_conteudoLimitado != limitar)
            {
                _conteudoLimitado = limitar;
                ConteudoPagina.HorizontalOptions = limitar ? LayoutOptions.Center : LayoutOptions.Fill;
                ConteudoPagina.WidthRequest = limitar ? LarguraMaximaConteudo : -1;
            }

            // Define a faixa atual: desktop, tablet/janela dividida ou largura compacta.
            var faixa = Width >= 1024 ? 2 : Width >= 640 ? 1 : 0;
            if (_ultimaFaixaLargura == faixa)
                return;

            // Guarda a faixa para evitar alterações repetidas durante pequenos redimensionamentos.
            _ultimaFaixaLargura = faixa;

            // Valores equivalem aos breakpoints de 16, 24 e 32px usados no CSS da página Web.
            ConteudoPagina.Padding = faixa switch
            {
                2 => new Thickness(32, 32, 32, 64),
                1 => new Thickness(24, 28, 24, 56),
                _ => new Thickness(16, 20, 16, 48)
            };
        }
    }
}
