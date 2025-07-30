using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Main.Nino
{
    public class CrearNinoDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        public string? NumeroMatricula { get; set; }

        [Required(ErrorMessage = "Debe especificar un responsable de pago")]
        public int ResponsablePagoId { get; set; }

        public List<string> IngredientesAlergicos { get; set; } = new List<string>();
    }
}
