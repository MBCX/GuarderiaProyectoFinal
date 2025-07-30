using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Menu.Ingrediente
{
    public class IngredienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del ingrediente es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        public bool EsAlergeno { get; set; }
        public string? Cantidad { get; set; } // Para relación con platos
    }
}
