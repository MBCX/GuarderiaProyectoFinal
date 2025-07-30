using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Menu.Consumo
{
    public class ValidacionMenuResultDto
    {
        public bool EsSeguro { get; set; }
        public List<string> IngredientesProblematicos { get; set; } = new List<string>();
        public string? MensajeValidacion { get; set; }
    }
}
