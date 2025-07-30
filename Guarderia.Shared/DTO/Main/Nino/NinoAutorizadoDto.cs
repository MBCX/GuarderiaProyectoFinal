using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Nino
{
    public class NinoAutorizadoDto
    {
        public int NinoId { get; set; }
        public string NinoNombre { get; set; } = string.Empty;
        public string NumeroMatricula { get; set; } = string.Empty;
        public DateTime FechaAutorizacion { get; set; }
        public DateTime? FechaRevocacion { get; set; }
        public bool Activa { get; set; }
        public string? Relacion { get; set; }
        public string? Observaciones { get; set; }
    }
}
