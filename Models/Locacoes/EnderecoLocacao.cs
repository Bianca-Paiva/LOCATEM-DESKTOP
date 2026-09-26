namespace LOCATEM_DESKTOP.Models.Locacoes
{
    /// <summary>Endereço de entrega informado pelo locatário na solicitação.</summary>
    public class EnderecoLocacao
    {
        public string Cep { get; set; } = string.Empty;
        public string RuaAvenida { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;

        public string TextoExibicao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RuaAvenida))
                    return "Endereço não informado";

                var complemento = string.IsNullOrWhiteSpace(Complemento)
                    ? string.Empty
                    : $" — {Complemento}";

                return $"{RuaAvenida}, {Numero}{complemento} — SP";
            }
        }
    }
}
