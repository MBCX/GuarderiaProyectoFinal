using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IPersonaAutorizadaService
    {
        Task<List<PersonaAutorizada>> ObtenerPersonasAutorizadasPorNiñoAsync(int ninoId);
        Task<List<PersonaAutorizada>> ObtenerPersonasActivasPorNiñoAsync(int ninoId);
        Task<PersonaAutorizada> ObtenerPorCedulaAsync(string cedula);
        Task<int> RegistrarPersonaAutorizadaAsync(PersonaAutorizada personaAutorizada);
        Task ActualizarPersonaAutorizadaAsync(PersonaAutorizada personaAutorizada);
        Task AutorizarParaNiñoAsync(int ninoId, string cedula, string relacion);
        Task RevocarAutorizacionAsync(int ninoId, string cedula, DateTime fechaRevocacion);
        Task<bool> ValidarPersonaAutorizadaAsync(string cedula, int ninoId);
        Task<bool> ValidarDatosObligatoriosPersonaAsync(PersonaAutorizada persona);
        Task<bool> PuedeRecogerNiñoAsync(string cedula, int ninoId);
        Task EliminarPersonaAutorizadaAsync(int id);
    }
}
