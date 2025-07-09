using System;

namespace Guarderia.Domain.Entities
{
    public class Asistencia
    {
        public int Id { get; set; }
        public int NinoId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Asistio { get; set; }

        // Relación inversa
        public Nino Nino { get; set; }
    }
}
