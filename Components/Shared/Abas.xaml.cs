using System.Collections;
using System.Windows.Input;

namespace LOCATEM_DESKTOP.Components.Shared
{
    /// <summary>
    /// Barra de abas de filtro com contador — migrada de
    /// components/Ferramentas/MinhasFerramentas/Abas/Abas.tsx.
    ///
    /// Genérica: recebe a lista de <see cref="AbaItem"/> já pronta e devolve a chave da aba
    /// tocada em <see cref="SelecionarCommand"/>. Pode ser reutilizada por Gerenciar Locações
    /// e Histórico de Locações quando forem migradas (mesmo uso do Abas.tsx no React).
    /// </summary>
    public partial class Abas : ContentView
    {
        public Abas()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(Abas));

        /// <summary>Lista de <see cref="AbaItem"/> exibida na barra.</summary>
        public IEnumerable? ItemsSource
        {
            get => (IEnumerable?)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelecionarCommandProperty =
            BindableProperty.Create(
                nameof(SelecionarCommand),
                typeof(ICommand),
                typeof(Abas));

        /// <summary>Executado ao tocar numa aba; o parâmetro é a <see cref="AbaItem.Chave"/>.</summary>
        public ICommand? SelecionarCommand
        {
            get => (ICommand?)GetValue(SelecionarCommandProperty);
            set => SetValue(SelecionarCommandProperty, value);
        }

        private void OnAbaTapped(object? sender, TappedEventArgs e)
        {
            if (sender is BindableObject { BindingContext: AbaItem aba })
            {
                SelecionarCommand?.Execute(aba.Chave);
            }
        }
    }
}
