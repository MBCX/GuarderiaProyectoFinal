using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Finanzas
{
    public class CargoMensualDto
    {
        public int Id { get; set; }
        public int NinoId { get; set; }
        public string NinoNombre { get; set; } = string.Empty;
        public string NumeroMatricula { get; set; } = string.Empty;
        public int ResponsablePagoId { get; set; }
        public string ResponsablePagoNombre { get; set; } = string.Empty;
        public int Mes { get; set; }
        public int Año { get; set; }
        public decimal CostoFijo { get; set; }
        public decimal CostoComidas { get; set; }
        public decimal TotalCargo { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Estado { get; set; } = "Pendiente";

        // Propiedades calculadas
        public string MesAño => $"{Mes:D2}/{Año}";
        public bool EstaPagado => Estado == "Pagado";
        public int DiasVencido => EstaPagado ? 0 : (DateTime.Now - FechaGeneracion).Days;
    }
}
