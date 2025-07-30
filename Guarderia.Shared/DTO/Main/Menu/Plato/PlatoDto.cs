using System.ComponentModel.DataAnnotations;
using Guarderia.Shared.DTO.Main.Menu.Ingrediente;

namespace Guarderia.Shared.DTO.Main.Menu.Plato
{
    public class PlatoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del plato es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de plato no puede exceder 50 caracteres")]
        public string? TipoPlato { get; set; }

        public List<IngredienteDto> Ingredientes { get; set; } = new List<IngredienteDto>();
        public List<string> NombresIngredientes { get; set; } = new List<string>();
        public int OrdenEnMenu { get; set; }
        public bool EsPlatoPrincipal { get; set; }
    }
}
