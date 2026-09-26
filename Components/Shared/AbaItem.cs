namespace LOCATEM_DESKTOP.Components.Shared
{
    /// <summary>
    /// Item da barra de abas de filtro — equivalente a AbaItem&lt;T&gt; de
    /// components/Ferramentas/MinhasFerramentas/Abas/Abas.tsx.
    ///
    /// É imutável: sempre que o filtro muda, o ViewModel monta uma nova lista de abas
    /// (nova contagem e nova aba ativa) e o componente Abas se redesenha.
    /// </summary>
    /// <param name="Chave">Identificador da aba (devolvido ao SelecionarCommand).</param>
    /// <param name="Label">Texto exibido, ex.: "Disponíveis".</param>
    /// <param name="Contagem">Número exibido na pílula de contador, ex.: 9.</param>
    /// <param name="Ativa">Aba atualmente selecionada.</param>
    public record AbaItem(string Chave, string Label, int Contagem, bool Ativa);
}
