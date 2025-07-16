using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    public class ResponsablePago
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string CuentaCorriente { get; set; } // Para realizar el cargo

        public List<Nino> NinosAPagar { get; set; }

        // También puede estar autorizado para recoger niños
        public List<NinoPersonaAutorizada> NinosAutorizados { get; set; }
    }
}
