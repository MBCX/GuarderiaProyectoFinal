using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IResponsablePagoRepository
    {
        Task<ResponsablePago> ObtenerPorIdAsync(int id);
        Task<ResponsablePago> ObtenerPorCedulaAsync(string cedula);
        Task<List<ResponsablePago>> ObtenerTodosAsync();
        Task<List<ResponsablePago>> ObtenerPorNinoIdAsync(int ninoId);
        Task AgregarAsync(ResponsablePago responsablePago);
        Task ActualizarAsync(ResponsablePago responsablePago);
        Task EliminarAsync(int id);
        Task<bool> ExistePorCedulaAsync(string cedula);
        Task<bool> TieneCuentaCorrienteValidaAsync(int id);
    }
}
