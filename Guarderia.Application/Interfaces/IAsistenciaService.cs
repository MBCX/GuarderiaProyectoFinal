using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IAsistenciaService
    {
        Task<List<Asistencia>> ObtenerAsistenciasPorNiñoAsync(int ninoId);
        Task<List<Asistencia>> ObtenerAsistenciasPorFechaAsync(DateTime fecha);
        Task<List<Asistencia>> ObtenerAsistenciasPorMesAsync(int mes, int año);
        Task<Asistencia> ObtenerAsistenciaPorNiñoYFechaAsync(int ninoId, DateTime fecha);
        Task RegistrarAsistenciaAsync(int ninoId, DateTime fecha, bool asistio);
        Task ActualizarAsistenciaAsync(Asistencia asistencia);
        Task<int> CalcularDiasAsistenciaDelMesAsync(int ninoId, int mes, int año);
        Task<decimal> CalcularPorcentajeAsistenciaAsync(int ninoId, int mes, int año);
        Task<decimal> CalcularCostoAsistenciaMensualAsync(int ninoId, int mes, int año);
        Task<bool> YaRegistradaAsync(int ninoId, DateTime fecha);
        Task<List<Asistencia>> GenerarReporteAsistenciaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Asistencia>> GenerarReporteAsistenciaMensualAsync(int mes, int año);
        Task EliminarAsistenciaAsync(int id);
        Task<int> ContarAsistentesDelDiaAsync(DateTime fecha);
    }
}
