using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Alergia
{
    public class AlergiaDto
    {
        public int Id { get; set; }
        public int NinoId { get; set; }
        public string NinoNombre { get; set; } = string.Empty;
        public int IngredienteId { get; set; }
        public string IngredienteNombre { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}
