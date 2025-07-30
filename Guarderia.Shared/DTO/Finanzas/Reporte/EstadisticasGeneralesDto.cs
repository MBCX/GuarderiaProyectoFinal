using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Finanzas.Reporte
{
    public class EstadisticasGeneralesDto
    {
        public int TotalNinosActivos { get; set; }
        public decimal PromedioAsistencia { get; set; }
        public int TotalAsistenciasRegistradas { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }
}
