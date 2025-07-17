using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface ICargoMensualService
    {
        Task<CargoMensual> ObtenerPorIdAsync(int id);
        Task<List<CargoMensual>> ObtenerPorNiñoAsync(int ninoId);
        Task<CargoMensual> ObtenerPorNiñoYMesAsync(int ninoId, int mes, int año);
        Task<List<CargoMensual>> ObtenerPorMesAsync(int mes, int año);
        Task<List<CargoMensual>> ObtenerPendientesAsync();
        Task<List<CargoMensual>> ObtenerPorResponsableAsync(int responsablePagoId);
        Task<int> GenerarCargoMensualAsync(int ninoId, int mes, int año);
        Task<CargoMensual> CalcularCargoMensualAsync(int ninoId, int mes, int año);
        Task ActualizarCargoAsync(CargoMensual cargo);
        Task MarcarComoPagadoAsync(int cargoId, DateTime fechaPago);
        Task<decimal> ObtenerTotalPendientePorResponsableAsync(int responsablePagoId);
        Task<bool> YaExisteCargoAsync(int ninoId, int mes, int año);
        Task RegenerarCargosDelMesAsync(int mes, int año);
        Task<List<CargoMensual>> GenerarReporteCargosAsync(int mes, int año);
    }
}
