using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IConsumoMenuService
    {
        Task<List<ConsumoMenu>> ObtenerPorNiñoAsync(int ninoId);
        Task<List<ConsumoMenu>> ObtenerPorFechaAsync(DateTime fecha);
        Task<ConsumoMenu> ObtenerPorNiñoYFechaAsync(int ninoId, DateTime fecha);
        Task<List<ConsumoMenu>> ObtenerPorMesAsync(int ninoId, int mes, int año);
        Task<int> RegistrarConsumoAsync(int ninoId, int menuId, DateTime fecha);
        Task ActualizarConsumoAsync(ConsumoMenu consumo);
        Task<bool> ValidarConsumoParaNiñoAsync(int ninoId, int menuId);
        Task<decimal> CalcularCostoMenusMensualAsync(int ninoId, int mes, int año);
        Task<int> ContarDiasComidasMensualAsync(int ninoId, int mes, int año);
        Task<List<ConsumoMenu>> GenerarReporteConsumoDiarioAsync(DateTime fecha);
        Task<List<ConsumoMenu>> GenerarReporteConsumoMensualAsync(int mes, int año);
        Task EliminarConsumoAsync(int id);
    }
}
