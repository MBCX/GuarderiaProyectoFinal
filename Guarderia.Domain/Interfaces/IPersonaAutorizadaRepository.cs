using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IPersonaAutorizadaRepository
    {
        Task<PersonaAutorizada> ObtenerPorIdAsync(int id);
        Task<PersonaAutorizada> ObtenerPorCedulaAsync(string cedula);
        Task<List<PersonaAutorizada>> ObtenerTodasAsync();
        Task<List<PersonaAutorizada>> ObtenerPorNinoIdAsync(int ninoId);
        Task<List<PersonaAutorizada>> ObtenerActivasPorNinoIdAsync(int ninoId);
        Task AgregarAsync(PersonaAutorizada personaAutorizada);
        Task ActualizarAsync(PersonaAutorizada personaAutorizada);
        Task EliminarAsync(int id);
        Task<bool> ExistePorCedulaAsync(string cedula);
        Task<bool> EstaAutorizadaParaNinoAsync(string cedula, int ninoId);
    }
}
