using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Nino
{
    public class NinoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de matrícula es obligatorio")]
        [StringLength(50, ErrorMessage = "El número de matrícula no puede exceder 50 caracteres")]
        public string NumeroMatricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        public DateTime FechaIngreso { get; set; }

        public DateTime? FechaBaja { get; set; }

        public bool Activo { get; set; } = true;

        public int? ResponsablePagoId { get; set; }

        // Propiedades de navegación
        public ResponsablePagoDto? ResponsablePago { get; set; }
        public List<AlergiaDto> Alergias { get; set; } = new List<AlergiaDto>();
        public List<PersonaAutorizadaDto> PersonasAutorizadas { get; set; } = new List<PersonaAutorizadaDto>();

        // Propiedades calculadas
        public int EdadAnios => DateTime.Now.Year - FechaNacimiento.Year;
        public int DiasEnGuarderia => (DateTime.Now - FechaIngreso).Days;
    }
}
