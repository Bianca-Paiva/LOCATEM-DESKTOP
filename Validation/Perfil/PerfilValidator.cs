using LOCATEM_DESKTOP.Helpers.Auth;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Validation.Auth;

namespace LOCATEM_DESKTOP.Validation.Perfil
{
    /// <summary>
    /// Migração do validation/Perfil/perfilSchema.ts. Mantém as regras de nome, telefone,
    /// documento (CPF/CNPJ conforme o perfil), CEP e endereço usadas no React.
    /// </summary>
    public static class PerfilValidator
    {
        public static PerfilFieldErrors Validate(PerfilFormSnapshot data)
        {
            var errors = new PerfilFieldErrors();

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

            if (string.IsNullOrWhiteSpace(data.Telefone))
                errors.Telefone = "O telefone é obrigatório";
            else if (!MaskHelper.ValidatePhone(data.Telefone))
                errors.Telefone = "Digite um telefone válido com DDD";

            if (string.IsNullOrWhiteSpace(data.Documento))
            {
                errors.Documento = "O documento é obrigatório";
            }
            else
            {
                var valido = data.Tipo == TipoUsuario.Locador
                    ? DocumentValidator.IsValidCnpj(data.Documento)
                    : DocumentValidator.IsValidCpf(data.Documento);

                if (!valido)
                    errors.Documento = "Documento inválido";
            }

            if (string.IsNullOrWhiteSpace(data.Cep))
                errors.Cep = "O CEP é obrigatório";
            else if (!MaskHelper.ValidateCep(data.Cep))
                errors.Cep = "Digite um CEP válido";

            if (string.IsNullOrWhiteSpace(data.Logradouro))
                errors.Logradouro = "O endereço é obrigatório";

            if (string.IsNullOrWhiteSpace(data.Numero))
                errors.Numero = "O número é obrigatório";

            return errors;
        }
    }
}
