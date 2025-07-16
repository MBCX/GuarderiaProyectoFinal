using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    public class PlatoIngrediente
    {
        public int PlatoId { get; set; }
        public int IngredienteId { get; set; }
        public string Cantidad { get; set; } // Cantidad del ingrediente
        public bool EsAlergeno { get; set; } // Si es un alérgeno conocido

        // Relaciones
        public Plato Plato { get; set; }
        public Ingrediente Ingrediente { get; set; }
    }
}
