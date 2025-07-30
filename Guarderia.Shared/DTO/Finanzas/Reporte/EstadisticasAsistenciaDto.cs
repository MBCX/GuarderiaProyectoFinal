using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Finanzas.Reporte
{
    public class EstadisticasAsistenciaDto
    {
        public int NinoId { get; set; }
        public string NinoNombre { get; set; } = string.Empty;
        public int TotalDias { get; set; }
        public int DiasAsistidos { get; set; }
        public int DiasFaltados { get; set; }
        public decimal PorcentajeAsistencia { get; set; }
    }
}
