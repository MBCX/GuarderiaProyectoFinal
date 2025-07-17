using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IFacturacionService
    {
        // Generación de Cargos
        Task<CargoMensual> GenerarCargoMensualAsync(int ninoId, int mes, int año);
        Task GenerarCargosMasivosMensualAsync(int mes, int año);
        Task<CargoMensual> RecalcularCargoAsync(int cargoId);

        // Gestión de Pagos
        Task MarcarComoPagadoAsync(int cargoId, DateTime fechaPago, string metodoPago);
        Task MarcarComoPendienteAsync(int cargoId, string motivo);
        Task<decimal> CalcularTotalPendienteAsync(int responsablePagoId);

        // Cálculos Financieros
        Task<decimal> CalcularCostoAsistenciaAsync(int ninoId, int mes, int año);
        Task<decimal> CalcularCostoMenusAsync(int ninoId, int mes, int año);
        Task<decimal> CalcularCostoFijoAsync(int mes, int año);
        Task<decimal> CalcularDescuentosAsync(int ninoId, int mes, int año);

        // Reportes Financieros
        Task<List<CargoMensual>> GenerarEstadoCuentaAsync(int responsablePagoId, int año);
        Task<decimal> CalcularIngresosTotalesAsync(int mes, int año);
        Task<List<CargoMensual>> ObtenerFacturacionPendienteAsync();
        Task<Dictionary<int, decimal>> GenerarResumenFacturacionAsync(int año);
    }
}
