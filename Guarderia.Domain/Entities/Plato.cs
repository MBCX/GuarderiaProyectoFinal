using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    public class Plato
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string TipoPlato { get; set; }

        public List<PlatoIngrediente> Ingredientes { get; set; }

        // Relación con menús que lo incluyen
        public List<MenuPlato> Menus { get; set; }
    }
}
