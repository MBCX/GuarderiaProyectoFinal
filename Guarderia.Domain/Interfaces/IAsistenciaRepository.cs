using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IAsistenciaRepository
    {
        Task<Asistencia> ObtenerPorIdAsync(int id);
        Task<List<Asistencia>> ObtenerTodasAsync();
        Task<List<Asistencia>> ObtenerPorNinoIdAsync(int NinoId);
        Task<List<Asistencia>> ObtenerPorFechaAsync(DateTime fecha); // NUEVO
        Task<List<Asistencia>> ObtenerPorMesYAñoAsync(int mes, int año); // NUEVO
        Task<int> ContarAsistenciasMensualesAsync(int NinoId, int mes, int año); // NUEVO
        Task RegistrarAsync(Asistencia asistencia);
        Task ActualizarAsync(Asistencia asistencia); // NUEVO
        Task EliminarAsync(int id);
        Task<bool> YaRegistradaAsync(int NinoId, DateTime fecha); // NUEVO
        Task<decimal> CalcularPorcentajeAsistenciaAsync(int NinoId, int mes, int año); // NUEVO
    }
}
