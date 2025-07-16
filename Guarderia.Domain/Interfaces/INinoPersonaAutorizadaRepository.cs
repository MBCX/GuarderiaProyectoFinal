using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface INinoPersonaAutorizadaRepository
    {
        Task<List<NinoPersonaAutorizada>> ObtenerPorNinoIdAsync(int ninoId);
        Task<List<NinoPersonaAutorizada>> ObtenerPorPersonaIdAsync(int personaAutorizadaId);
        Task<List<NinoPersonaAutorizada>> ObtenerActivasPorNinoAsync(int ninoId);
        Task<NinoPersonaAutorizada> ObtenerRelacionAsync(int ninoId, int personaAutorizadaId);
        Task AgregarAsync(NinoPersonaAutorizada relacion);
        Task ActualizarAsync(NinoPersonaAutorizada relacion);
        Task RevocarAutorizacionAsync(int ninoId, int personaAutorizadaId, DateTime fechaRevocacion);
        Task<bool> EstaAutorizadaAsync(int ninoId, int personaAutorizadaId);
        Task<bool> PuedeRecogerNinoAsync(string cedulaPersona, int ninoId);
    }
}
