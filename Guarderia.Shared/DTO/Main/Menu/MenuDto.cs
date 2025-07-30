using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Menu
{
    public class MenuDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del menú es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal Precio { get; set; }

        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; } = true;

        public List<PlatoDto> Platos { get; set; } = new List<PlatoDto>();
        public List<string> IngredientesDelMenu { get; set; } = new List<string>();
    }
}
