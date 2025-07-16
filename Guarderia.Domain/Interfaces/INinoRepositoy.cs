using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface INinoRepository
    {
        Task<Nino> ObtenerPorIdAsync(int id);
        Task<Nino> ObtenerPorMatriculaAsync(string numeroMatricula);
        Task<List<Nino>> ObtenerTodosAsync();
        Task<List<Nino>> ObtenerActivosAsync();
        Task<List<Nino>> ObtenerInactivosAsync();
        Task<List<Nino>> ObtenerPorResponsablePagoAsync(int responsablePagoId);
        Task AgregarAsync(Nino nino);
        Task ActualizarAsync(Nino nino);
        Task EliminarAsync(int id);
        Task DarBajaAsync(int id, DateTime fechaBaja);
        Task ReactivarAsync(int id);
        Task<bool> ExisteMatriculaAsync(string numeroMatricula);
        Task<int> ContarActivosAsync();
    }
}
