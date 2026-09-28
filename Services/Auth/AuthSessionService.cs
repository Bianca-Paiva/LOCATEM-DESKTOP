using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <inheritdoc cref="IAuthSessionService"/>
    public class AuthSessionService : IAuthSessionService
    {
        public Usuario? UsuarioAtual { get; private set; }

        public bool IsAuthenticated => UsuarioAtual is not null;

        public event EventHandler? SessaoAlterada;

        // Mantém o usuário autenticado em memória e avisa as telas que dependem do estado da sessão.
        public void DefinirUsuario(Usuario usuario)
        {
            UsuarioAtual = usuario;
            SessaoAlterada?.Invoke(this, EventArgs.Empty);
        }

        // Limpa a sessão atual e notifica os componentes para refletirem o estado desautenticado.
        public void Logout()
        {
            UsuarioAtual = null;
            SessaoAlterada?.Invoke(this, EventArgs.Empty);
        }

        // Substitui os dados da sessão após uma edição de perfil, preservando uma única fonte de verdade.
        public void AtualizarUsuario(Usuario dadosAtualizados)
        {
            if (UsuarioAtual is null) return;
            UsuarioAtual = dadosAtualizados;
            SessaoAlterada?.Invoke(this, EventArgs.Empty);
        }
    }
}
