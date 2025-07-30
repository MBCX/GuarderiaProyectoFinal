using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Menu.Consumo
{
    public class ConsumoMenuDto
    {
        public int Id { get; set; }
        public int NinoId { get; set; }
        public string NinoNombre { get; set; } = string.Empty;
        public string NumeroMatricula { get; set; } = string.Empty;
        public int MenuId { get; set; }
        public string MenuNombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal CostoReal { get; set; }
        public string? Observaciones { get; set; }
    }
}
