using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IResponsablePagoService
    {
        Task<ResponsablePago> ObtenerPorIdAsync(int id);
        Task<ResponsablePago> ObtenerPorCedulaAsync(string cedula);
        Task<List<ResponsablePago>> ObtenerTodosAsync();
        Task<ResponsablePago> ObtenerPorNiñoAsync(int ninoId);
        Task<int> RegistrarResponsablePagoAsync(ResponsablePago responsablePago);
        Task ActualizarResponsablePagoAsync(ResponsablePago responsablePago);
        Task AsignarANiñoAsync(int responsablePagoId, int ninoId);
        Task<bool> ValidarDatosObligatoriosAsync(ResponsablePago responsable);
        Task<bool> ValidarCuentaCorrienteAsync(string cuentaCorriente);
        Task<decimal> ObtenerTotalPendienteAsync(int responsablePagoId);
        Task EliminarResponsablePagoAsync(int id);
    }
}
