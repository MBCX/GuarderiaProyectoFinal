using Guarderia.Shared.DTO.Main.Pago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Finanzas.Reporte.Reporte
{
    public class EstadoCuentaDto
    {
        public ResponsablePagoDto ResponsablePago { get; set; } = new();
        public List<CargoMensualDto> Cargos { get; set; } = new List<CargoMensualDto>();
        public decimal TotalPendiente { get; set; }
        public decimal TotalPagado { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public int Año { get; set; }
    }
}
