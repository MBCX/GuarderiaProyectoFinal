using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    internal class Nino
    {
        public string Nombre = String.Empty;
        public DateTime FechaNacimiento = new DateTime(1940, 1, 1);
        public string Matricula = String.Empty;
        public DateTime FechaIngreso = DateTime.MaxValue;

        // DateTime.MinValue indica que NO
        // esta de baja.
        public DateTime FechaDeBaja = DateTime.MinValue;
    }
}
