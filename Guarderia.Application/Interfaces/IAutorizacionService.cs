using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IAutorizacionService
    {
        // Gestión de Autorizaciones
        Task AutorizarPersonaParaNinoAsync(int ninoId, int personaAutorizadaId, string relacion);
        Task RevocarAutorizacionAsync(int ninoId, int personaAutorizadaId, DateTime fechaRevocacion, string motivo);
        Task<bool> EstaAutorizadaAsync(int ninoId, string cedula);
        Task<bool> EstaAutorizadaAsync(int ninoId, int personaAutorizadaId);

        // Consultas de Autorización
        Task<List<PersonaAutorizada>> ObtenerPersonasAutorizadasAsync(int ninoId);
        Task<List<PersonaAutorizada>> ObtenerPersonasActivasAsync(int ninoId);
        Task<List<Nino>> ObtenerNinosAutorizadosAsync(string cedula);

        // Validaciones de Recogida
        Task<bool> PuedeRecogerNinoAsync(string cedula, int ninoId);
        Task<PersonaAutorizada> ValidarYObtenerPersonaAutorizadaAsync(string cedula, int ninoId);
        Task RegistrarRecogidaAsync(int ninoId, string cedula, DateTime fechaHora);

        // Historial de Autorizaciones
        Task<List<NinoPersonaAutorizada>> ObtenerHistorialAutorizacionesAsync(int ninoId);
        Task<List<NinoPersonaAutorizada>> ObtenerAutorizacionesRevocadasAsync(int ninoId);
    }
}
