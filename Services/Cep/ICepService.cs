using LOCATEM_DESKTOP.Models.Cep;

namespace LOCATEM_DESKTOP.Services.Cep
{
    public interface ICepService
    {
        Task<CepResultado?> ConsultarAsync(string cep, CancellationToken cancellationToken = default);
    }
}
