using System.Windows.Input;
using MauiIcons.Core;
using MauiIcons.Material;

namespace LOCATEM_DESKTOP.Components.Layout
{
    /// <summary>
    /// Cabeçalho das telas internas — migrado de components/Layout/Header/Header.tsx, na variante
    /// desktop e já filtrado pelo perfil do locador (é o único perfil com telas internas migradas).
    ///
    /// Busca e carrinho não aparecem de propósito: no React eles também são ocultados para o locador,
    /// que não realiza locações.
    /// </summary>
    public partial class AppHeader : ContentView
    {
        private record NavItem(string Label, string Rota, MaterialIcons Icone);

        // Mesma lista (e ordem) dos itens visíveis ao locador no Header do React.
        private static readonly NavItem[] Itens =
        {
            new("Início", "homeLocador", MaterialIcons.Home),
            new("Minhas Ferramentas", "minhasFerramentas", MaterialIcons.Inventory),
            new("Gerenciar Locações", "gerenciarLocacoes", MaterialIcons.Assignment),
            new("Histórico", "historicoLocacoes", MaterialIcons.History),
            new("Avaliações", "avaliacao", MaterialIcons.Star),
            new("Notificações", "notificacoes", MaterialIcons.Notifications),
            new("Suporte", "suporte", MaterialIcons.SupportAgent)
        };

        public AppHeader()
        {
            InitializeComponent();
            MontarNavegacao();
        }

        public static readonly BindableProperty RotaAtualProperty =
            BindableProperty.Create(nameof(RotaAtual), typeof(string), typeof(AppHeader), string.Empty,
                propertyChanged: (b, _, _) => ((AppHeader)b).MontarNavegacao());
        public string RotaAtual { get => (string)GetValue(RotaAtualProperty); set => SetValue(RotaAtualProperty, value); }

        /// <summary>Recebe a rota do item tocado como parâmetro.</summary>
        public static readonly BindableProperty NavegarCommandProperty =
            BindableProperty.Create(nameof(NavegarCommand), typeof(ICommand), typeof(AppHeader),
                propertyChanged: (b, _, _) => ((AppHeader)b).MontarNavegacao());
        public ICommand? NavegarCommand { get => (ICommand?)GetValue(NavegarCommandProperty); set => SetValue(NavegarCommandProperty, value); }

        public static readonly BindableProperty NomeUsuarioProperty =
            BindableProperty.Create(nameof(NomeUsuario), typeof(string), typeof(AppHeader), string.Empty,
                propertyChanged: (b, _, v) => ((AppHeader)b).AvatarIniciais.Text = ExtrairIniciais(v as string));
        public string NomeUsuario { get => (string)GetValue(NomeUsuarioProperty); set => SetValue(NomeUsuarioProperty, value); }

        private void MontarNavegacao()
        {
            Navegacao.Clear();

            foreach (var item in Itens)
            {
                var ativo = string.Equals(item.Rota, RotaAtual, StringComparison.OrdinalIgnoreCase);
                var cor = ativo
                    ? (Color)Application.Current!.Resources["TextPrimaryColor"]
                    : (Color)Application.Current!.Resources["TextMutedColor"];

                var conteudo = new HorizontalStackLayout
                {
                    Spacing = 6,
                    Padding = new Thickness(10, 8),
                    Children =
                    {
                        new MauiIcon { Icon = item.Icone, IconSize = 20, IconColor = cor, VerticalOptions = LayoutOptions.Center },
                        new Label { Text = item.Label, FontSize = 13, TextColor = cor, VerticalOptions = LayoutOptions.Center,
                                    FontAttributes = ativo ? FontAttributes.Bold : FontAttributes.None }
                    }
                };

                var rota = item.Rota;
                var toque = new TapGestureRecognizer();
                toque.Tapped += (_, _) => NavegarCommand?.Execute(rota);
                conteudo.GestureRecognizers.Add(toque);

                Navegacao.Add(conteudo);
            }
        }

        /// <summary>Iniciais do nome, como o fallback do Avatar do React quando não há foto.</summary>
        private static string ExtrairIniciais(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return "?";

            var partes = nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length == 1
                ? partes[0][..1].ToUpperInvariant()
                : $"{partes[0][0]}{partes[^1][0]}".ToUpperInvariant();
        }
    }
}
