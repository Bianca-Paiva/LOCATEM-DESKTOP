using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOCATEM_DESKTOP.Models.Auth
{
    public class UsuarioMeResponse
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string Documento { get; set; } = string.Empty;

        public string TipoUsuario { get; set; } = string.Empty;

        public string? Endereco { get; set; }

        public int Desde { get; set; }

        public string? FotoUrl { get; set; }

        public ReputacaoMeResponse Reputacao { get; set; } = new();
    }

    public class ReputacaoMeResponse
    {
        public double Rating { get; set; }

        public int TotalAvaliacoes { get; set; }

        public int LocacoesConcluidas { get; set; }

        public double? EntregasNoPrazoPercentual { get; set; }
    }
}