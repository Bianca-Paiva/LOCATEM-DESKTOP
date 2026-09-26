namespace LOCATEM_DESKTOP.Models.Avaliacoes
{
    /// <summary>
    /// Identifica cada aspecto que pode receber nota no fluxo de avaliações.
    /// O locador utiliza Locatario, Entrega e Plataforma; os demais ficam disponíveis para futura migração do perfil locatário.
    /// </summary>
    public enum AspectoAvaliacao
    {
        Locador,
        Locatario,
        Entrega,
        Produto,
        Plataforma
    }

    /// <summary>
    /// Registro persistido na própria locação após o envio da avaliação.
    /// Mantém as subnotas, a média global e a observação exatamente no vínculo em que a experiência aconteceu.
    /// </summary>
    public sealed class RegistroAvaliacao
    {
        /// <summary>Notas individuais por aspecto avaliado.</summary>
        public Dictionary<AspectoAvaliacao, int> SubAvaliacoes { get; set; } = new();

        /// <summary>Média arredondada das subnotas, exibida nos cards de avaliações realizadas.</summary>
        public int NotaGlobal { get; set; }

        /// <summary>Comentário opcional informado pelo usuário.</summary>
        public string Observacao { get; set; } = string.Empty;
    }
}
