using LOCATEM_DESKTOP.Validation.Auth;

namespace LOCATEM_DESKTOP.Validation.Perfil
{
    /// <summary>Mensagens globais do formulário de edição, equivalentes às usadas no React.</summary>
    public static class PerfilMessages
    {
        public static readonly AlertaMessage Required =
            new("Campos obrigatórios", "Preencha todos os campos obrigatórios para continuar.");

        public static readonly AlertaMessage InvalidName =
            new("Nome inválido", "Por favor, digite seu nome completo.");

        public static readonly AlertaMessage InvalidPhone =
            new("Telefone inválido", "Digite um telefone válido com DDD.");

        public static readonly AlertaMessage InvalidCnpj =
            new("CNPJ inválido", "Digite seu CNPJ completo e válido.");

        public static readonly AlertaMessage InvalidCpf =
            new("CPF inválido", "Digite seu CPF completo e válido.");

        public static readonly AlertaMessage InvalidCep =
            new("CEP inválido", "Digite um CEP válido.");

        public static readonly AlertaMessage CepNaoEncontrado =
            new("CEP não encontrado", "Verifique o CEP informado e tente novamente.");

        public static readonly AlertaMessage CepErro =
            new("Erro ao buscar CEP", "Não foi possível consultar o CEP. Tente novamente.");

        public static readonly AlertaMessage SaveError =
            new("Não foi possível salvar", "Confira sua conexão e tente novamente.");
    }
}
