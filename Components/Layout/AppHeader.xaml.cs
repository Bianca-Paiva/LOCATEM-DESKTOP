using System.Windows.Input;
using MauiIcons.Core;
using MauiIcons.Material;
using MauiIcons.Material.Outlined;

namespace LOCATEM_DESKTOP.Components.Layout
{
    /// <summary>
    /// Cabeçalho das telas internas.
    /// Exibe somente as opções disponíveis para o locador.
    /// </summary>
    public partial class AppHeader : ContentView
    {
        private record NavItem(
            string Label,
            string Rota,
            Enum IconeOutlined,
            Enum IconePreenchido);

        private static readonly NavItem[] Itens =
        {
            new(
                "Início",
                "homeLocador",
                MaterialOutlinedIcons.Home,
                MaterialIcons.Home),

            new(
                "Minhas Ferramentas",
                "minhasFerramentas",
                MaterialOutlinedIcons.Inventory2,
                MaterialIcons.Inventory2),

            new(
                "Gerenciar Locações",
                "gerenciarLocacoes",
                MaterialOutlinedIcons.Assignment,
                MaterialIcons.Assignment),

            new(
                "Histórico",
                "historicoLocacoes",
                MaterialOutlinedIcons.History,
                MaterialIcons.History),

            new(
                "Avaliações",
                "avaliacao",
                MaterialOutlinedIcons.StarBorder,
                MaterialIcons.Star),

            new(
                "Notificações",
                "notificacoes",
                MaterialOutlinedIcons.Notifications,
                MaterialIcons.Notifications),

            new(
                "Suporte",
                "suporte",
                MaterialOutlinedIcons.HeadsetMic,
                MaterialIcons.HeadsetMic)
        };

        public AppHeader()
        {
            InitializeComponent();

            MontarNavegacao();
        }

        // =========================================================
        // ROTA ATUAL
        // =========================================================

        public static readonly BindableProperty RotaAtualProperty =
            BindableProperty.Create(
                nameof(RotaAtual),
                typeof(string),
                typeof(AppHeader),
                string.Empty,
                propertyChanged: (bindable, _, _) =>
                    ((AppHeader)bindable).MontarNavegacao());

        public string RotaAtual
        {
            get => (string)GetValue(RotaAtualProperty);
            set => SetValue(RotaAtualProperty, value);
        }

        // =========================================================
        // COMANDO DE NAVEGAÇÃO
        // =========================================================

        public static readonly BindableProperty NavegarCommandProperty =
            BindableProperty.Create(
                nameof(NavegarCommand),
                typeof(ICommand),
                typeof(AppHeader),
                propertyChanged: (bindable, _, _) =>
                    ((AppHeader)bindable).MontarNavegacao());

        public ICommand? NavegarCommand
        {
            get => (ICommand?)GetValue(NavegarCommandProperty);
            set => SetValue(NavegarCommandProperty, value);
        }

        // =========================================================
        // NOME DO USUÁRIO
        // =========================================================

        public static readonly BindableProperty NomeUsuarioProperty =
            BindableProperty.Create(
                nameof(NomeUsuario),
                typeof(string),
                typeof(AppHeader),
                string.Empty,
                propertyChanged: (bindable, _, novoValor) =>
                {
                    var header = (AppHeader)bindable;

                    header.AvatarIniciais.Text =
                        ExtrairIniciais(novoValor as string);
                });

        public string NomeUsuario
        {
            get => (string)GetValue(NomeUsuarioProperty);
            set => SetValue(NomeUsuarioProperty, value);
        }

        // =========================================================
        // NAVEGAÇÃO
        // =========================================================

        private void MontarNavegacao()
        {
            Navegacao.Clear();

            // Todos os itens da navegação ficam pretos.
            var corTexto =
                (Color)Application.Current!
                    .Resources["TextPrimaryColor"];

            foreach (var item in Itens)
            {
                var ativo =
                    string.Equals(
                        item.Rota,
                        RotaAtual,
                        StringComparison.OrdinalIgnoreCase);

                // -------------------------------------------------
                // ÍCONE
                // -------------------------------------------------

                var icone = new MauiIcon
                {
                    Icon = ativo
                        ? item.IconePreenchido
                        : item.IconeOutlined,

                    IconSize = 20,
                    IconColor = corTexto,
                    VerticalOptions = LayoutOptions.Center
                };

                // -------------------------------------------------
                // TEXTO
                // -------------------------------------------------

                var label = new Label
                {
                    Text = item.Label,
                    FontSize = 13,
                    TextColor = corTexto,
                    VerticalOptions = LayoutOptions.Center,

                    // Ativo = negrito
                    FontAttributes =
                        ativo
                            ? FontAttributes.Bold
                            : FontAttributes.None
                };

                // -------------------------------------------------
                // CONTEÚDO DO ITEM
                // -------------------------------------------------

                var conteudo = new HorizontalStackLayout
                {
                    Spacing = 6,

                    Padding =
                        new Thickness(
                            10,
                            8,
                            10,
                            7),

                    VerticalOptions =
                        LayoutOptions.Center,

                    Children =
                    {
                        icone,
                        label
                    }
                };

                // -------------------------------------------------
                // LINHA INFERIOR
                // -------------------------------------------------

                var linhaAtiva = new BoxView
                {
                    HeightRequest = 2,        
                    
                    BackgroundColor =
                    ativo
                        ? corTexto
                        : Colors.Transparent,

                    HorizontalOptions = LayoutOptions.Fill,
                    Margin = new Thickness(10, 0)
                };

                // -------------------------------------------------
                // ITEM COMPLETO
                // -------------------------------------------------

                var container = new VerticalStackLayout
                {
                    Spacing = 0,

                    Children =
                    {
                        conteudo,
                        linhaAtiva
                    }
                };

                // -------------------------------------------------
                // CLIQUE
                // -------------------------------------------------

                var rota = item.Rota;

                var toque =
                    new TapGestureRecognizer();

                toque.Tapped += (_, _) =>
                {
                    NavegarCommand?.Execute(rota);
                };

                container
                    .GestureRecognizers
                    .Add(toque);

                Navegacao.Add(container);
            }
        }

        // =========================================================
        // AVATAR
        // =========================================================

        private static string ExtrairIniciais(
            string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return "?";

            var partes =
                nome
                    .Trim()
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length == 1)
            {
                return partes[0][..1]
                    .ToUpperInvariant();
            }

            return
                $"{partes[0][0]}{partes[^1][0]}"
                    .ToUpperInvariant();
        }
    }
}