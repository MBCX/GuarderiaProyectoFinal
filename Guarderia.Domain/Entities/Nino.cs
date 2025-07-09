using System.Collections.Generic;

namespace Guarderia.Domain.Entities
{
    public class Nino
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Relación con asistencias
        public List<Asistencia> Asistencias { get; set; }

        // Relación con comidas
        public List<Comida> Comidas { get; set; }

        public DateTime FechaIngreso = DateTime.Now;

        public DateTime FechaNacimiento = new DateTime(1980, 1, 1);

        public DateTime? FechaBaja = null;
    }
}
