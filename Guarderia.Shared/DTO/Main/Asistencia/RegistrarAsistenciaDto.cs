using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Asistencia
{
    public class RegistrarAsistenciaDto
    {
        [Required(ErrorMessage = "El ID del niño es obligatorio")]
        public int NinoId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        public bool Asistio { get; set; } = true;
    }
}
