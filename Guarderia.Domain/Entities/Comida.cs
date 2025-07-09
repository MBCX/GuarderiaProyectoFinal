using System;

namespace Guarderia.Domain.Entities
{
    public class Comida
    {
        public int Id { get; set; }
        public int NinoId { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } // Desayuno, almuerzo, etc.
        public decimal Costo { get; set; }

        // Relación inversa
        public Nino Nino { get; set; }
    }
}
