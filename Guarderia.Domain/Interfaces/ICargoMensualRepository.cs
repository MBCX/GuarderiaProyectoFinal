using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface ICargoMensualRepository
    {
        Task<CargoMensual> ObtenerPorIdAsync(int id);
        Task<List<CargoMensual>> ObtenerTodosAsync();
        Task<List<CargoMensual>> ObtenerPorNinoIdAsync(int ninoId);
        Task<List<CargoMensual>> ObtenerPorResponsableAsync(int responsablePagoId);
        Task<CargoMensual> ObtenerPorNinoYMesAsync(int ninoId, int mes, int año);
        Task<List<CargoMensual>> ObtenerPorMesYAñoAsync(int mes, int año);
        Task<List<CargoMensual>> ObtenerPendientesAsync();
        Task GenerarAsync(CargoMensual cargoMensual);
        Task ActualizarAsync(CargoMensual cargoMensual);
        Task MarcarComoPagadoAsync(int id, DateTime fechaPago);
        Task<decimal> ObtenerTotalPendientePorResponsableAsync(int responsablePagoId);
    }
}
