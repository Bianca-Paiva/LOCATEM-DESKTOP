namespace LOCATEM_DESKTOP.Validation.Auth
{
    /// <summary>Título + mensagem exibidos no componente AlertaBanner (migrado de Alerta.tsx).</summary>
    public class AlertaMessage
    {
        public string Titulo { get; }
        public string? Mensagem { get; }

        public AlertaMessage(string titulo, string? mensagem = null)
        {
            Titulo = titulo;
            Mensagem = mensagem;
        }
    }
}
