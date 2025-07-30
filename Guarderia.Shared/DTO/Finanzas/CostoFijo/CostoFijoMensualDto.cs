using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.Finanzas.CostoFijo
{
    public class CostoFijoMensualDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La fecha de vigencia es obligatoria")]
        public DateTime FechaVigenciaDesde { get; set; }

        public DateTime? FechaVigenciaHasta { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; }

        // Propiedades calculadas
        public bool EstaVigente => (
            Activo && FechaVigenciaDesde <= DateTime.Now &&
            (FechaVigenciaHasta == null || FechaVigenciaHasta >= DateTime.Now)
        );
    }
}
