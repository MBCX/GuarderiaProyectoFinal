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
        Task RegistrarAsistenciaAsync(int ninoId, DateTime fecha, bool asistio);
        Task<int> CalcularDiasAsistenciaDelMesAsync(int ninoId, int mes, int año);
        Task<decimal> CalcularCostoAsistenciaMensualAsync(int ninoId, int mes, int año);
    }
}
