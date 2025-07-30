using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.PersonaAutorizada
{
    public class AutorizarPersonaDto
    {
        [Required(ErrorMessage = "El ID del niño es obligatorio")]
        public int NinoId { get; set; }

        [Required(ErrorMessage = "La cédula de la persona es obligatoria")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "La relación es obligatoria")]
        [StringLength(50, ErrorMessage = "La relación no puede exceder 50 caracteres")]
        public string Relacion { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? Observaciones { get; set; }
    }
}
