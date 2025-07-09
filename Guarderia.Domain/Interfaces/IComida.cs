using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IComidaRepository
    {
        Task<Comida> ObtenerPorIdAsync(int id);
        Task<List<Comida>> ObtenerTodasAsync();
        Task<List<Comida>> ObtenerPorNiñoIdAsync(int niñoId);
        Task RegistrarAsync(Comida comida);
        Task EliminarAsync(int id);
    }
}
