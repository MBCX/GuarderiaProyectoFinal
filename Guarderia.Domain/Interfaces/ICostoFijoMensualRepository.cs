using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface ICostoFijoMensualRepository
    {
        Task<CostoFijoMensual> ObtenerPorIdAsync(int id);
        Task<List<CostoFijoMensual>> ObtenerTodosAsync();
        Task<CostoFijoMensual> ObtenerActivoAsync();
        Task<CostoFijoMensual> ObtenerPorFechaAsync(DateTime fecha);
        Task<List<CostoFijoMensual>> ObtenerHistorialAsync();
        Task AgregarAsync(CostoFijoMensual costoFijo);
        Task ActualizarAsync(CostoFijoMensual costoFijo);
        Task DesactivarAsync(int id);
        Task<decimal> ObtenerMontoVigenteAsync(DateTime fecha);
    }
}
