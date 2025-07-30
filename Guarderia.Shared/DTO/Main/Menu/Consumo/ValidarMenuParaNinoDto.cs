using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Menu.Consumo
{
    public class ValidarMenuParaNinoDto
    {
        [Required(ErrorMessage = "El ID del niño es obligatorio")]
        public int NinoId { get; set; }

        [Required(ErrorMessage = "El ID del menú es obligatorio")]
        public int MenuId { get; set; }
    }
}
