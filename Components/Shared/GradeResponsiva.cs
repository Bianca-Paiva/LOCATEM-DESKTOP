using System.Collections;
using System.Collections.Specialized;

namespace LOCATEM_DESKTOP.Components.Shared
{
    /// <summary>
    /// Grade responsiva de cards — equivalente ao
    /// <c>grid-template-columns: repeat(auto-fill, minmax(210px, 1fr))</c> usado pela versão Web
    /// em Minhas Ferramentas.
    ///
    /// Calcula quantas colunas cabem na largura REAL disponível (cada coluna com pelo menos
    /// <see cref="LarguraMinimaItem"/>), divide a largura igualmente entre elas e quebra os itens
    /// em linhas. A última linha mantém a mesma largura de coluna das demais (como o auto-fill do
    /// CSS), em vez de esticar os cards restantes.
    ///
    /// Uso: informe ItemsSource e ItemTemplate (DataTemplate). Cada item recebe o seu objeto
    /// como BindingContext.
    /// </summary>
    public class GradeResponsiva : ContentView
    {
        private readonly VerticalStackLayout _linhas = new();

        private INotifyCollectionChanged? _colecaoObservada;
        private int _colunas;

        public GradeResponsiva()
        {
            _linhas.Spacing = Espacamento;
            Content = _linhas;

            SizeChanged += (_, _) => AtualizarColunas();
        }

        // =========================================================
        // ITEMS SOURCE
        // =========================================================

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(GradeResponsiva),
                propertyChanged: (bindable, antigo, novo) =>
                    ((GradeResponsiva)bindable).AoTrocarItemsSource(
                        antigo as IEnumerable,
                        novo as IEnumerable));

        public IEnumerable? ItemsSource
        {
            get => (IEnumerable?)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        // =========================================================
        // ITEM TEMPLATE
        // =========================================================

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(GradeResponsiva),
                propertyChanged: (bindable, _, _) =>
                    ((GradeResponsiva)bindable).Reconstruir());

        public DataTemplate? ItemTemplate
        {
            get => (DataTemplate?)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        // =========================================================
        // LARGURA MÍNIMA DE CADA ITEM (minmax(210px, 1fr))
        // =========================================================

        public static readonly BindableProperty LarguraMinimaItemProperty =
            BindableProperty.Create(
                nameof(LarguraMinimaItem),
                typeof(double),
                typeof(GradeResponsiva),
                210d,
                propertyChanged: (bindable, _, _) =>
                    ((GradeResponsiva)bindable).AtualizarColunas());

        public double LarguraMinimaItem
        {
            get => (double)GetValue(LarguraMinimaItemProperty);
            set => SetValue(LarguraMinimaItemProperty, value);
        }

        // =========================================================
        // ESPAÇAMENTO ENTRE ITENS (gap)
        // =========================================================

        public static readonly BindableProperty EspacamentoProperty =
            BindableProperty.Create(
                nameof(Espacamento),
                typeof(double),
                typeof(GradeResponsiva),
                20d,
                propertyChanged: (bindable, _, novo) =>
                {
                    var grade = (GradeResponsiva)bindable;

                    grade._linhas.Spacing = (double)novo;
                    grade._colunas = 0;
                    grade.AtualizarColunas();
                });

        public double Espacamento
        {
            get => (double)GetValue(EspacamentoProperty);
            set => SetValue(EspacamentoProperty, value);
        }

        // =========================================================
        // LÓGICA
        // =========================================================

        private void AoTrocarItemsSource(
            IEnumerable? antigo,
            IEnumerable? novo)
        {
            if (_colecaoObservada is not null)
            {
                _colecaoObservada.CollectionChanged -= AoAlterarColecao;
                _colecaoObservada = null;
            }

            if (novo is INotifyCollectionChanged observavel)
            {
                _colecaoObservada = observavel;
                _colecaoObservada.CollectionChanged += AoAlterarColecao;
            }

            Reconstruir();
        }

        private void AoAlterarColecao(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            Reconstruir();
        }

        /// <summary>
        /// Recalcula o número de colunas para a largura atual. Só reconstrói a grade quando
        /// esse número muda — pequenas variações de largura apenas redimensionam as colunas
        /// (que são * / proporcionais).
        /// </summary>
        private void AtualizarColunas()
        {
            if (Width <= 0)
            {
                return;
            }

            var colunas =
                Math.Max(
                    1,
                    (int)Math.Floor(
                        (Width + Espacamento) /
                        (LarguraMinimaItem + Espacamento)));

            if (colunas == _colunas)
            {
                return;
            }

            _colunas = colunas;

            Reconstruir();
        }

        private void Reconstruir()
        {
            _linhas.Children.Clear();

            if (_colunas < 1 ||
                ItemTemplate is null ||
                ItemsSource is null)
            {
                return;
            }

            var itens = ItemsSource.Cast<object>().ToList();

            for (var inicio = 0; inicio < itens.Count; inicio += _colunas)
            {
                var linha =
                    new Grid
                    {
                        ColumnSpacing = Espacamento
                    };

                for (var coluna = 0; coluna < _colunas; coluna++)
                {
                    linha.ColumnDefinitions.Add(
                        new ColumnDefinition(GridLength.Star));
                }

                for (var coluna = 0;
                     coluna < _colunas && inicio + coluna < itens.Count;
                     coluna++)
                {
                    if (ItemTemplate.CreateContent() is not View visual)
                    {
                        continue;
                    }

                    visual.BindingContext = itens[inicio + coluna];

                    Grid.SetColumn(visual, coluna);
                    linha.Children.Add(visual);
                }

                _linhas.Children.Add(linha);
            }
        }
    }
}
