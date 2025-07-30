using Guarderia.Shared.DTO.Main.Asistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Finanzas.Reporte
{
    public class ReporteAsistenciaDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<AsistenciaDto> Asistencias { get; set; } = new List<AsistenciaDto>();
        public Dictionary<int, EstadisticasAsistenciaDto> EstadisticasPorNino { get; set; } = new();
        public EstadisticasGeneralesDto EstadisticasGenerales { get; set; } = new();
    }
}
