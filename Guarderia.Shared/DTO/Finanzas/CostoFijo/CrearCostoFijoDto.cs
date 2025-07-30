using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Finanzas.CostoFijo
{
    public class CrearCostoFijoDto
    {
        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La fecha de vigencia es obligatoria")]
        public DateTime FechaVigencia { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
