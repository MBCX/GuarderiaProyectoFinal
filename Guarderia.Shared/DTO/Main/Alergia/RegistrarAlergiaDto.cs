using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Alergia
{
    public class RegistrarAlergiaDto
    {
        [Required(ErrorMessage = "El ID del niño es obligatorio")]
        public int NinoId { get; set; }

        [Required(ErrorMessage = "El ingrediente es obligatorio")]
        public string Ingrediente { get; set; } = string.Empty;
    }
}
