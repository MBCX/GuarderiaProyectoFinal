using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Asistencia
{
    public class AsistenciaDto
    {
        public int Id { get; set; }
        public int NinoId { get; set; }
        public string NinoNombre { get; set; } = string.Empty;
        public string NumeroMatricula { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Asistio { get; set; }
    }
}
