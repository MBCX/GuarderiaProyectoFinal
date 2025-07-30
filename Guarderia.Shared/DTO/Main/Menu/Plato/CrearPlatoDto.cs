using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Menu.Plato
{
    public class CrearPlatoDto
    {
        [Required(ErrorMessage = "El nombre del plato es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de plato no puede exceder 50 caracteres")]
        public string? TipoPlato { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos un ingrediente")]
        public List<int> IngredienteIds { get; set; } = new List<int>();
    }
}
