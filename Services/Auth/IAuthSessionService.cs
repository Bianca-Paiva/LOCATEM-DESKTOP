using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <summary>
    /// Equivalente ao AuthContext/AuthProvider do React: guarda o usuário autenticado da sessão
    /// atual do app. Registrado como singleton no DI (MauiProgram.cs) para ficar acessível a
    /// qualquer ViewModel, como o contexto React fica acessível via useAuth().
    /// </summary>
    public interface IAuthSessionService
    {
        /// <summary>Usuário autenticado, ou null quando não há sessão.</summary>
        Usuario? UsuarioAtual { get; }

        bool IsAuthenticated { get; }

        /// <summary>Disparado sempre que UsuarioAtual muda (login, logout ou atualização de dados).</summary>
        event EventHandler? SessaoAlterada;

        /// <summary>
        /// Autentica pelo e-mail digitado no login (mesmo comportamento do AuthProvider: resolve
        /// pelo catálogo mockado, com fallback para um usuário novo).
        /// </summary>
        Usuario Login(string email);

        void Logout();

        /// <summary>Atualiza campos do usuário logado (usado pelo modal "Editar Perfil").</summary>
        void AtualizarUsuario(Usuario dadosAtualizados);
    }
}
