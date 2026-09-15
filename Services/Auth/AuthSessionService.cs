using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <inheritdoc cref="IAuthSessionService"/>
    public class AuthSessionService : IAuthSessionService
    {
        public Usuario? UsuarioAtual { get; private set; }

        public bool IsAuthenticated => UsuarioAtual is not null;

        public event EventHandler? SessaoAlterada;

        public Usuario Login(string email)
        {
            // O fluxo de Login ainda não está integrado a um backend real (ver AuthService,
            // cuja chamada fica comentada/não usada no LoginViewModel), então resolvemos o
            // usuário a partir do catálogo mockado, com fallback para um usuário novo.
            var usuarioEncontrado = UsuariosMock.BuscarPorEmail(email) ?? UsuariosMock.CriarFallback(email);
            UsuarioAtual = usuarioEncontrado;
            SessaoAlterada?.Invoke(this, EventArgs.Empty);
            return usuarioEncontrado;
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
