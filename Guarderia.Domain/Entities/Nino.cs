using System.Collections.Generic;

namespace Guarderia.Domain.Entities
{
    public class Nino
    {
        public int Id { get; set; }
        public string NumeroMatricula { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaBaja { get; set; }
        public bool Activo { get; set; }

        public int? ResponsablePagoId { get; set; }
        public ResponsablePago ResponsablePago { get; set; }

        // Relaciones existentes
        public List<Asistencia> Asistencias { get; set; }
        public List<Comida> Comidas { get; set; }
        public List<Alergia> Alergias { get; set; }

        // Nuevas relaciones
        public List<NinoPersonaAutorizada> PersonasAutorizadas { get; set; }
        public List<ConsumoMenu> ConsumosMenu { get; set; }
        public List<CargoMensual> CargosMensuales { get; set; }
    }
}
