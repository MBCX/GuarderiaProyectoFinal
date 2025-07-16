using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IComidaRepository
    {
        Task<Comida> ObtenerPorIdAsync(int id);
        Task<List<Comida>> ObtenerTodasAsync();
        Task<List<Comida>> ObtenerPorNinoIdAsync(int NinoId);
        Task<List<Comida>> ObtenerPorFechaAsync(DateTime fecha);
        Task<List<Comida>> ObtenerPorTipoAsync(string tipo);
        Task<List<Comida>> ObtenerPorMesYAñoAsync(int mes, int año);
        Task<decimal> CalcularCostoComidasMensualAsync(int NinoId, int mes, int año);
        Task RegistrarAsync(Comida comida);
        Task ActualizarAsync(Comida comida);
        Task EliminarAsync(int id);
        Task<bool> YaRegistradaAsync(int NinoId, DateTime fecha, string tipo);
        Task<int> ContarComidasMensualesAsync(int NinoId, int mes, int año);
    }
}
