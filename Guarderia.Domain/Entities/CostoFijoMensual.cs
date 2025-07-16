using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    public class CostoFijoMensual
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
