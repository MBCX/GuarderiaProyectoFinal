using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IConsumoMenuRepository
    {
        Task<ConsumoMenu> ObtenerPorIdAsync(int id);
        Task<List<ConsumoMenu>> ObtenerTodosAsync();
        Task<List<ConsumoMenu>> ObtenerPorNinoIdAsync(int ninoId);
        Task<List<ConsumoMenu>> ObtenerPorFechaAsync(DateTime fecha);
        Task<List<ConsumoMenu>> ObtenerPorNinoYMesAsync(int ninoId, int mes, int año);
        Task<ConsumoMenu> ObtenerPorNinoYFechaAsync(int ninoId, DateTime fecha);
        Task RegistrarAsync(ConsumoMenu consumoMenu);
        Task ActualizarAsync(ConsumoMenu consumoMenu);
        Task EliminarAsync(int id);
        Task<decimal> CalcularCostoMenusMensualAsync(int ninoId, int mes, int año);
        Task<int> ContarDiasComidasMensualAsync(int ninoId, int mes, int año);
    }
}
