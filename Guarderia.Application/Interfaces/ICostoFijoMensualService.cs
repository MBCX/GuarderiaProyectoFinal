using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface ICostoFijoMensualService
    {
        Task<CostoFijoMensual> ObtenerActivoAsync();
        Task<decimal> ObtenerMontoVigenteAsync(DateTime fecha);
        Task<List<CostoFijoMensual>> ObtenerHistorialAsync();
        Task<int> CrearCostoFijoAsync(decimal monto, DateTime fechaVigencia, string descripcion);
        Task ActualizarCostoFijoAsync(CostoFijoMensual costoFijo);
        Task ActivarNuevoCostoAsync(int id, DateTime fechaVigencia);
        Task DesactivarCostoAsync(int id);
        Task<bool> ValidarMontoAsync(decimal monto);
        Task<bool> TieneCostoVigenteAsync(DateTime fecha);
    }
}
