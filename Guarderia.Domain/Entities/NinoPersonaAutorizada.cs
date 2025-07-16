using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    public class NinoPersonaAutorizada
    {
        public int NinoId { get; set; }
        public int PersonaAutorizadaId { get; set; }
        public DateTime FechaAutorizacion { get; set; }
        public DateTime? FechaRevocacion { get; set; }
        public bool Activa { get; set; }
        public string Observaciones { get; set; }

        // Relaciones
        public Nino Nino { get; set; }
        public PersonaAutorizada PersonaAutorizada { get; set; }
    }
}
