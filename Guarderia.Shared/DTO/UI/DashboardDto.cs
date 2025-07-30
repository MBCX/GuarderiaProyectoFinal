using Guarderia.Shared.DTO.Finanzas;
using Guarderia.Shared.DTO.Finanzas.Reporte;
using Guarderia.Shared.DTO.Main.Asistencia;
using Guarderia.Shared.DTO.Main.Menu.Consumo;
using Guarderia.Shared.DTO.Main.Nino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared.DTO.UI
{
    public class DashboardDto
    {
        public EstadisticasGeneralesDto EstadisticasGenerales { get; set; } = new();
        public List<AsistenciaDto> AsistenciaDelDia { get; set; } = new List<AsistenciaDto>();
        public List<ConsumoMenuDto> ConsumosDelDia { get; set; } = new List<ConsumoMenuDto>();
        public List<CargoMensualDto> CargosPendientes { get; set; } = new List<CargoMensualDto>();
        public List<NinoDto> NinosConAlergias { get; set; } = new List<NinoDto>();
        public decimal IngresosMesActual { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
