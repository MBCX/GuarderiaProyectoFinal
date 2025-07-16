using Guarderia.Domain.Entities;

namespace Guarderia.Application.Common.Interfaces
{
    public interface IConsumoDiarioRepository
    {
        Task<List<ConsumoDiario>> ObtenerTodosAsync();
        Task<ConsumoDiario?> ObtenerPorIdAsync(int id);
        Task<List<ConsumoDiario>> ObtenerPorNinoIdAsync(int ninoId);
        Task<List<ConsumoDiario>> ObtenerPorFechaAsync(DateTime fecha);
        Task<List<ConsumoDiario>> ObtenerPorMesYAnoAsync(int mes, int ano);
        Task<decimal> CalcularTotalPorNinoYMesAsync(int ninoId, int mes, int ano);
        Task AgregarAsync(ConsumoDiario consumo);
        Task ActualizarAsync(ConsumoDiario consumo);
        Task EliminarAsync(int id);
        Task<bool> ExisteRegistroAsync(int ninoId, DateTime fecha);
        Task<ConsumoDiario> ObtenerPorNinoYFechaAsync(int ninoId, DateTime fecha);
    }
}
