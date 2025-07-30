using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Finanzas.Reporte.Reporte
{
    public class ReporteFinancieroDto
    {
        public int Mes { get; set; }
        public int Año { get; set; }
        public List<CargoMensualDto> Cargos { get; set; } = new List<CargoMensualDto>();
        public decimal TotalGenerado { get; set; }
        public decimal TotalCobrado { get; set; }
        public decimal TotalPendiente { get; set; }
        public int CargosPagados { get; set; }
        public int CargosPendientes { get; set; }
        public decimal PorcentajeCobro => TotalGenerado > 0 ? (TotalCobrado / TotalGenerado) * 100 : 0;
    }
}
