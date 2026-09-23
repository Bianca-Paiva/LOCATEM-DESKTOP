using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOCATEM_DESKTOP.Models.Auth
{
    public class LoginResponse
    {
        public string Mensagem { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}