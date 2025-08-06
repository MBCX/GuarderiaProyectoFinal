using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    public class MenuPlato
    {
        public int Id { get; set; }
        public int PlatoId { get; set; }
        public int Orden { get; set; } // Orden del plato en el menú
        public bool EsPlatoPrincipal { get; set; }

        // Relaciones
        public Menu Menu { get; set; }
        public Plato Plato { get; set; }
    }
}
