using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    public class CargoMensual
    {
        public int Id { get; set; }
        public int NinoId { get; set; }
        public int ResponsablePagoId { get; set; }
        public int Mes { get; set; }
        public int Año { get; set; }
        public decimal CostoFijo { get; set; }
        public decimal CostoComidas { get; set; }
        public decimal TotalCargo { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Estado { get; set; } // Pendiente, Pagado, Vencido

        // Relaciones
        public Nino Nino { get; set; }
        public ResponsablePago ResponsablePago { get; set; }
    }
}
