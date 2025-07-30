using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Finanzas
{
    public class GenerarCargoMensualDto
    {
        [Required(ErrorMessage = "El ID del niño es obligatorio")]
        public int NinoId { get; set; }

        [Required(ErrorMessage = "El mes es obligatorio")]
        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12")]
        public int Mes { get; set; }

        [Required(ErrorMessage = "El año es obligatorio")]
        [Range(2020, 2030, ErrorMessage = "El año debe estar entre 2020 y 2030")]
        public int Año { get; set; }
    }
}
