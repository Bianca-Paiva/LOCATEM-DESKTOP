using LOCATEM_DESKTOP.Models.Auth;

namespace LOCATEM_DESKTOP.Validation.Auth
{
    /// <summary>
    /// Migrado de validation/Cadastro/cadastroSchema.ts (schema zod). Centraliza as regras de
    /// validação do Cadastro em vez de espalhá-las pelo ViewModel, como pedido na migração.
    /// </summary>
    public static class CadastroValidator
    {
        public static CadastroFieldErrors Validate(CadastroFormSnapshot data)
        {
            var errors = new CadastroFieldErrors();

            // nome: obrigatório + precisa ter nome e sobrenome (>= 2 palavras, cada uma com >= 2 letras)
            if (string.IsNullOrWhiteSpace(data.Nome))
            {
                errors.Nome = "O nome é obrigatório";
            }
            else
            {
                var partes = data.Nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length < 2 || partes.Any(p => p.Length < 2))
                    errors.Nome = "Por favor, digite seu nome completo";
            }

            // email
            if (string.IsNullOrWhiteSpace(data.Email))
                errors.Email = "O e-mail é obrigatório.";
            else if (!EmailValidator.IsValid(data.Email))
                errors.Email = "Digite um e-mail válido.";

            // telefone
            if (string.IsNullOrWhiteSpace(data.Telefone))
                errors.Telefone = "O telefone é obrigatório";
            else if (!Helpers.Auth.MaskHelper.ValidatePhone(data.Telefone))
                errors.Telefone = "Digite um telefone válido com DDD";

            // documento (obrigatório + checksum, dependendo do tipo)
            if (string.IsNullOrWhiteSpace(data.Documento))
            {
                errors.Documento = "O documento é obrigatório";
            }
            else
            {
                var isCnpj = data.Tipo == TipoUsuario.Locador;
                var valido = isCnpj ? DocumentValidator.IsValidCnpj(data.Documento) : DocumentValidator.IsValidCpf(data.Documento);
                if (!valido) errors.Documento = "Documento inválido";
            }

            // cep
            if (string.IsNullOrWhiteSpace(data.Cep))
                errors.Cep = "O CEP é obrigatório";
            else if (!Helpers.Auth.MaskHelper.ValidateCep(data.Cep))
                errors.Cep = "Digite um CEP válido";

            // logradouro / numero
            if (string.IsNullOrWhiteSpace(data.Logradouro)) errors.Logradouro = "O endereço é obrigatório";
            if (string.IsNullOrWhiteSpace(data.Numero)) errors.Numero = "O número é obrigatório";

            // senha: 8+ caracteres, minúscula, maiúscula, número e caractere especial
            if (string.IsNullOrEmpty(data.Senha))
            {
                errors.Senha = "A senha é obrigatória";
            }
            else if (!PasswordValidator.CheckPasswordStrength(data.Senha).IsStrong)
            {
                // Mesmo comportamento do schema original: mensagem vazia (o alerta de força
                // fraca/média é quem comunica o problema — ver PasswordMessages).
                errors.Senha = string.Empty;
            }

            // confirmarSenha
            if (string.IsNullOrEmpty(data.ConfirmarSenha))
                errors.ConfirmarSenha = "A confirmação de senha é obrigatória";
            else if (data.Senha != data.ConfirmarSenha)
                errors.ConfirmarSenha = "As senhas não coincidem";

            return errors;
        }
    }
}
