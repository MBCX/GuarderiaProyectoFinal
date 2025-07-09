using Guarderia.Domain.Entities;

namespace Guarderia.Application.Common.Interfaces
{
    public interface IConsumoDiarioRepository
    {
        Task<List<ConsumoDiario>> ObtenerTodosAsync();
        Task<ConsumoDiario?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(ConsumoDiario consumo);
        Task ActualizarAsync(ConsumoDiario consumo);
        Task EliminarAsync(int id);
    }
}
