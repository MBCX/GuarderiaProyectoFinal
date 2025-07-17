using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IReporteService
    {
        // Reportes de Asistencia
        Task<List<Asistencia>> GenerarReporteAsistenciaDiariaAsync(DateTime fecha);
        Task<List<Asistencia>> GenerarReporteAsistenciaMensualAsync(int mes, int año);
        Task<Dictionary<int, decimal>> GenerarReportePorcentajeAsistenciaAsync(int mes, int año);

        // Reportes de Consumo
        Task<List<ConsumoMenu>> GenerarReporteConsumoMenuDiarioAsync(DateTime fecha);
        Task<List<ConsumoMenu>> GenerarReporteConsumoMenuMensualAsync(int mes, int año);
        Task<List<Comida>> GenerarReporteConsumoComidasAsync(DateTime fechaInicio, DateTime fechaFin);

        // Reportes Financieros
        Task<List<CargoMensual>> GenerarReporteCargosAsync(int mes, int año);
        Task<List<CargoMensual>> GenerarReportePendientesPagoAsync();
        Task<decimal> GenerarReporteIngresosMensualesAsync(int mes, int año);

        // Reportes de Alergias
        Task<List<Nino>> GenerarReporteNiñosConAlergiasAsync();
        Task<Dictionary<string, int>> GenerarReporteEstadisticasAlergiasAsync();

        // Reportes Generales
        Task<Dictionary<string, object>> GenerarDashboardEstadisticasAsync();
        Task<List<Nino>> GenerarReporteNiñosActivosAsync();
        Task<List<Nino>> GenerarReporteNiñosInactivosAsync();
    }
}
