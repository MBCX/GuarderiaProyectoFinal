using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface INiñoRepository
    {
        Task<Nino> ObtenerPorIdAsync(int id);
        Task<List<Nino>> ObtenerTodosAsync();
        Task AgregarAsync(Nino nino);
        Task ActualizarAsync(Nino nino);
        Task EliminarAsync(int id);
    }
}
