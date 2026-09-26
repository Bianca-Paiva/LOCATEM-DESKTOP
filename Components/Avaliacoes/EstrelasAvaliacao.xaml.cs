using System.Windows.Input;
using LOCATEM_DESKTOP.Models.Avaliacoes;

namespace LOCATEM_DESKTOP.Components.Avaliacoes
{
    /// <summary>
    /// Fileira reutilizável de cinco estrelas.
    /// Continua atendendo os usos somente leitura já existentes e passa a aceitar clique quando Interativa for verdadeira.
    /// </summary>
    public partial class EstrelasAvaliacao : ContentView
    {
        // A quantidade é fixa para manter a escala de avaliação de 1 a 5 usada em todo o projeto.
        private const int QuantidadeEstrelas = 5;

        // As cores seguem a identidade visual atual do Desktop: amarelo da marca para nota preenchida e cinza para nota vazia.
        private static readonly Color CorAtiva = Color.FromArgb("#FFCA00");
        private static readonly Color CorInativa = Color.FromArgb("#D9D9D9");

        // A lista mantém referência às Labels para atualizar cor, tamanho e cursor visual sem recriar o componente.
        private readonly List<Label> _estrelas = new();

        public EstrelasAvaliacao()
        {
            InitializeComponent();
            CriarEstrelas();
            AplicarEstadoVisual();
        }

        // Nota atual exibida pela fileira.
        public static readonly BindableProperty NotaProperty = BindableProperty.Create(
            nameof(Nota),
            typeof(double),
            typeof(EstrelasAvaliacao),
            0d,
            propertyChanged: OnEstadoVisualChanged);

        public double Nota
        {
            get => (double)GetValue(NotaProperty);
            set => SetValue(NotaProperty, value);
        }

        // Tamanho das estrelas para adaptar o componente aos cards e ao modal.
        public static readonly BindableProperty TamanhoEstrelaProperty = BindableProperty.Create(
            nameof(TamanhoEstrela),
            typeof(double),
            typeof(EstrelasAvaliacao),
            14d,
            propertyChanged: OnEstadoVisualChanged);

        public double TamanhoEstrela
        {
            get => (double)GetValue(TamanhoEstrelaProperty);
            set => SetValue(TamanhoEstrelaProperty, value);
        }

        // Quando falso, os gestos continuam presentes internamente, mas não executam nenhuma ação.
        public static readonly BindableProperty InterativaProperty = BindableProperty.Create(
            nameof(Interativa),
            typeof(bool),
            typeof(EstrelasAvaliacao),
            false,
            propertyChanged: OnEstadoVisualChanged);

        public bool Interativa
        {
            get => (bool)GetValue(InterativaProperty);
            set => SetValue(InterativaProperty, value);
        }

        // Command executado com SelecaoEstrela para informar contexto e valor em um único parâmetro.
        public static readonly BindableProperty SelecionarNotaCommandProperty = BindableProperty.Create(
            nameof(SelecionarNotaCommand),
            typeof(ICommand),
            typeof(EstrelasAvaliacao));

        public ICommand? SelecionarNotaCommand
        {
            get => (ICommand?)GetValue(SelecionarNotaCommandProperty);
            set => SetValue(SelecionarNotaCommandProperty, value);
        }

        // Contexto livre usado pelo chamador para identificar produto, aspecto ou qualquer outro item avaliado.
        public static readonly BindableProperty ContextoProperty = BindableProperty.Create(
            nameof(Contexto),
            typeof(object),
            typeof(EstrelasAvaliacao));

        public object? Contexto
        {
            get => GetValue(ContextoProperty);
            set => SetValue(ContextoProperty, value);
        }

        // Cria cada estrela uma única vez e associa o valor correspondente ao gesto de toque.
        private void CriarEstrelas()
        {
            for (var indice = 0; indice < QuantidadeEstrelas; indice++)
            {
                var notaSelecionada = indice + 1;
                var estrela = new Label
                {
                    Text = "★",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };

                var toque = new TapGestureRecognizer();
                toque.Tapped += (_, _) => SelecionarNota(notaSelecionada);
                estrela.GestureRecognizers.Add(toque);

                _estrelas.Add(estrela);
                Fileira.Add(estrela);
            }
        }

        // Executa a seleção apenas quando o componente está explicitamente configurado como interativo.
        private void SelecionarNota(int nota)
        {
            if (!Interativa || SelecionarNotaCommand is null)
                return;

            var parametro = new SelecaoEstrela(Contexto, nota);
            if (SelecionarNotaCommand.CanExecute(parametro))
                SelecionarNotaCommand.Execute(parametro);
        }

        // Callback único para propriedades que alteram somente a apresentação das estrelas.
        private static void OnEstadoVisualChanged(BindableObject bindable, object oldValue, object newValue) =>
            ((EstrelasAvaliacao)bindable).AplicarEstadoVisual();

        // Atualiza todas as estrelas sem substituir controles ou perder os GestureRecognizers existentes.
        private void AplicarEstadoVisual()
        {
            for (var indice = 0; indice < _estrelas.Count; indice++)
            {
                var estrela = _estrelas[indice];
                estrela.FontSize = TamanhoEstrela;
                estrela.TextColor = indice < Nota ? CorAtiva : CorInativa;
                estrela.Opacity = Interativa ? 1d : 0.95d;
            }
        }
    }
}
