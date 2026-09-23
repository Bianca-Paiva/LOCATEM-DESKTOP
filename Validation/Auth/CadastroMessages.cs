namespace LOCATEM_DESKTOP.Validation.Auth
{
    /// <summary>Migrado de validation/Cadastro/cadastroMessages.ts.</summary>
    public static class CadastroMessages
    {
        public static readonly AlertaMessage Required = new("Campos obrigatórios", "Preencha todos os campos obrigatórios para continuar.");
        public static readonly AlertaMessage InvalidName = new("Nome inválido", "Por favor, digite seu nome completo.");
        public static readonly AlertaMessage InvalidEmail = new("E-mail inválido", "Digite um endereço de e-mail válido.");
        public static readonly AlertaMessage InvalidPhone = new("Telefone inválido", "Digite um telefone válido com DDD.");
        public static readonly AlertaMessage InvalidCnpj = new("CNPJ inválido", "Digite seu CNPJ completo.");
        public static readonly AlertaMessage InvalidCep = new("CEP inválido", "Digite um CEP válido.");
        public static readonly AlertaMessage Success = new("Sucesso", "Conta criada com sucesso!");
        public static readonly AlertaMessage ApiError = new("Erro", "Erro ao conectar com a API.");
    }
}