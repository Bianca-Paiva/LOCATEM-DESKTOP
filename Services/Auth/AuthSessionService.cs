using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <inheritdoc cref="IAuthSessionService"/>
    public class AuthSessionService : IAuthSessionService
    {
        public Usuario? UsuarioAtual { get; private set; }

        public bool IsAuthenticated => UsuarioAtual is not null;

        public event EventHandler? SessaoAlterada;

        public void DefinirUsuario(Usuario usuario)
        {
            UsuarioAtual = usuario;
            SessaoAlterada?.Invoke(this, EventArgs.Empty);
        }

        public void Logout()
        {
            UsuarioAtual = null;
            SessaoAlterada?.Invoke(this, EventArgs.Empty);
        }

        public void AtualizarUsuario(Usuario dadosAtualizados)
        {
            if (UsuarioAtual is null) return;
            UsuarioAtual = dadosAtualizados;
            SessaoAlterada?.Invoke(this, EventArgs.Empty);
        }
    }
}
