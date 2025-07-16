using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }

        public List<MenuPlato> Platos { get; set; }

        public List<ConsumoMenu> Consumos { get; set; }
    }
}
