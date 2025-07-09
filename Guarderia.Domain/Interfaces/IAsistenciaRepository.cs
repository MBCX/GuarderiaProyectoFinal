using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IAsistenciaRepository
    {
        Task<Asistencia> ObtenerPorIdAsync(int id);
        Task<List<Asistencia>> ObtenerTodasAsync();
        Task<List<Asistencia>> ObtenerPorNiñoIdAsync(int niñoId);
        Task RegistrarAsync(Asistencia asistencia);
        Task EliminarAsync(int id);
    }
}
