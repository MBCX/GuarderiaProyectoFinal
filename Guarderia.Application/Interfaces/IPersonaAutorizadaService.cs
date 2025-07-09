using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Application.Interfaces
{
    public interface IPersonaAutorizadaService
    {
        Task<List<dynamic>> ObtenerPersonasAutorizadasPorNiñoAsync(int ninoId);
        Task RegistrarPersonaAutorizadaAsync(int ninoId, string cedula, string nombre,
            string direccion, string telefono, string relacion);
        Task EliminarPersonaAutorizadaAsync(int ninoId, string cedula);
        Task<bool> ValidarPersonaAutorizadaAsync(string cedula, int ninoId);
        Task<bool> ValidarDatosObligatoriosPersonaAsync(string cedula, string nombre,
            string direccion, string telefono, string relacion);
    }
}
