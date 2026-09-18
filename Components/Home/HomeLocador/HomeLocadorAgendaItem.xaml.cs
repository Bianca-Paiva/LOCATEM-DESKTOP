using MauiIcons.Material;
using LOCATEM_DESKTOP.Helpers.Formatacao;
using LOCATEM_DESKTOP.Models.Home;

namespace LOCATEM_DESKTOP.Components.Home.HomeLocador
{
    /// <summary>
    /// Linha de um evento logístico da "Agenda da Semana" — migrado de HomeLocadorAgendaItem.tsx.
    /// </summary>
    public partial class HomeLocadorAgendaItem : ContentView
    {
        /// <summary>
        /// Textos, ícone e cores de cada movimento. Como a entrega é feita por transportadora
        /// terceirizada, evitamos "Retirada"/"Devolução" e deixamos o sentido do trajeto explícito.
        /// </summary>
        private record MovimentoVisual(string Label, string Complemento, MaterialIcons Icone, string Cor, string Fundo);

        private static readonly Dictionary<TipoMovimentoLogisticoLocador, MovimentoVisual> Config = new()
        {
            [TipoMovimentoLogisticoLocador.ColetaParaEntrega] =
                new("Coleta para entrega", "Transportadora indo buscar com você", MaterialIcons.LocalShipping, "#137333", "#E6F4EA"),
            [TipoMovimentoLogisticoLocador.RetornoAoLocador] =
                new("Retorno ao locador", "Transportadora trazendo de volta", MaterialIcons.Undo, "#BA1A1A", "#FFDAD6")
        };

        public HomeLocadorAgendaItem()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is not AgendaSemanaLocadorItem evento) return;

            Dia.Text = FormatoDataBr.FormatarDia(evento.Data);
            Mes.Text = FormatoDataBr.FormatarMes(evento.Data).ToUpperInvariant();

            var config = Config[evento.TipoMovimento];
            var cor = Color.FromArgb(config.Cor);

            MovimentoIcone.Icon = config.Icone;
            MovimentoIcone.IconColor = cor;
            MovimentoLabel.Text = config.Label;
            MovimentoLabel.TextColor = cor;
            MovimentoComplemento.Text = config.Complemento;
            MovimentoComplemento.TextColor = cor;
            Movimento.BackgroundColor = Color.FromArgb(config.Fundo);
        }
    }
}
