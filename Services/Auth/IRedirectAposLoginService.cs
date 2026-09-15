namespace LOCATEM_DESKTOP.Services.Auth
{
    /// <summary>
    /// Migrado de utils/Auth/redirectAposLogin.ts: ponto único para marcar/ler/limpar a rota de
    /// redirecionamento pós-login (ex.: usuário deslogado tenta continuar para o pagamento a
    /// partir do Carrinho, é levado ao Login e, ao concluir, deve voltar exatamente para lá).
    ///
    /// Usa armazenamento em memória (válido apenas durante a sessão atual do app), o mesmo
    /// espírito do sessionStorage usado no React. Registrado como singleton no DI.
    ///
    /// NOTA DE ESCOPO: as rotas "carrinho" e "produtoDetalhe" (ROTAS_VALIDAS no React) ainda não
    /// foram migradas para o MAUI nesta tarefa (fora do escopo de Login/Cadastro), então por ora
    /// nenhuma tela chama MarcarRedirect — o serviço já fica pronto para quando essas telas forem
    /// migradas, assim como pedido pela tarefa ("mantenha a arquitetura pronta").
    /// </summary>
    public interface IRedirectAposLoginService
    {
        void MarcarRedirect(string rota);
        string? LerRedirect();
        void LimparRedirect();
    }
}
