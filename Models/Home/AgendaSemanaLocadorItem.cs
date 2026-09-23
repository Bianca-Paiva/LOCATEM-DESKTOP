namespace LOCATEM_DESKTOP.Models.Home
{
    /// <summary>Evento da seção "Agenda da Semana", derivado de uma locação real do locador.</summary>
    public class AgendaSemanaLocadorItem
    {
        /// <summary>Único por evento (uma locação pode gerar coleta e retorno).</summary>
        public string Id { get; set; } = string.Empty;

        public string LocacaoId { get; set; } = string.Empty;

        /// <summary>Data do evento, formato "dd/mm/aaaa".</summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>Horário do evento, ex: "09:00".</summary>
        public string Hora { get; set; } = string.Empty;

        public string Ferramenta { get; set; } = string.Empty;
        public string Imagem { get; set; } = string.Empty;
        public string Locatario { get; set; } = string.Empty;
        public TipoMovimentoLogisticoLocador TipoMovimento { get; set; }
    }
}
