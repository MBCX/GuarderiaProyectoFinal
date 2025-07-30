using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Application.Interfaces;
using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;

namespace Guarderia.Application.Services
{
    public class AutorizacionService : IAutorizacionService
    {
        private readonly INinoPersonaAutorizadaRepository _ninoPersonaRepository;
        private readonly IPersonaAutorizadaRepository _personaAutorizadaRepository;
        private readonly INinoRepository _ninoRepository;

        public AutorizacionService(
            INinoPersonaAutorizadaRepository ninoPersonaRepository,
            IPersonaAutorizadaRepository personaAutorizadaRepository,
            INinoRepository ninoRepository)
        {
            _ninoPersonaRepository = ninoPersonaRepository;
            _personaAutorizadaRepository = personaAutorizadaRepository;
            _ninoRepository = ninoRepository;
        }

        public async Task AutorizarPersonaParaNinoAsync(int ninoId, int personaAutorizadaId, string relacion)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("El niño especificado no existe");
            }

            if (!nino.Activo)
            {
                throw new InvalidOperationException("No se puede autorizar personas para un niño inactivo");
            }

            var persona = await _personaAutorizadaRepository.ObtenerPorIdAsync(personaAutorizadaId);
            if (persona == null)
            {
                throw new ArgumentException("La persona autorizada especificada no existe");
            }

            if (await EstaAutorizadaAsync(ninoId, personaAutorizadaId))
            {
                throw new InvalidOperationException("La persona ya está autorizada para recoger a este niño");
            }

            var autorizacion = new NinoPersonaAutorizada
            {
                NinoId = ninoId,
                PersonaAutorizadaId = personaAutorizadaId,
                FechaAutorizacion = DateTime.Now,
                Activa = true,
                Observaciones = $"Relación: {relacion}"
            };

            await _ninoPersonaRepository.AgregarAsync(autorizacion);
        }

        public async Task RevocarAutorizacionAsync(int ninoId, int personaAutorizadaId, DateTime fechaRevocacion, string motivo)
        {
            var autorizacion = await _ninoPersonaRepository.ObtenerRelacionAsync(ninoId, personaAutorizadaId);
            if (autorizacion == null)
            {
                throw new ArgumentException("No existe autorización entre el niño y la persona especificada");
            }

            if (!autorizacion.Activa)
            {
                throw new InvalidOperationException("La autorización ya fue revocada anteriormente");
            }

            // Validar fecha de revocación
            if (fechaRevocacion < autorizacion.FechaAutorizacion)
            {
                throw new ArgumentException("La fecha de revocación no puede ser anterior a la fecha de autorización");
            }

            // Revocar la autorización
            autorizacion.FechaRevocacion = fechaRevocacion;
            autorizacion.Activa = false;
            autorizacion.Observaciones += $" | Revocada el {fechaRevocacion:dd/MM/yyyy}: {motivo}";

            await _ninoPersonaRepository.ActualizarAsync(autorizacion);
        }

        public async Task<bool> EstaAutorizadaAsync(int ninoId, string cedula)
        {
            return await _ninoPersonaRepository.PuedeRecogerNinoAsync(cedula, ninoId);
        }

        public async Task<bool> EstaAutorizadaAsync(int ninoId, int personaAutorizadaId)
        {
            return await _ninoPersonaRepository.EstaAutorizadaAsync(ninoId, personaAutorizadaId);
        }

        public async Task<List<PersonaAutorizada>> ObtenerPersonasAutorizadasAsync(int ninoId)
        {
            return await _personaAutorizadaRepository.ObtenerPorNinoIdAsync(ninoId);
        }

        public async Task<List<PersonaAutorizada>> ObtenerPersonasActivasAsync(int ninoId)
        {
            return await _personaAutorizadaRepository.ObtenerActivasPorNinoIdAsync(ninoId);
        }

        public async Task<List<Nino>> ObtenerNinosAutorizadosAsync(string cedula)
        {
            var persona = await _personaAutorizadaRepository.ObtenerPorCedulaAsync(cedula);
            if (persona == null)
            {
                return new List<Nino>();
            }

            var autorizaciones = await _ninoPersonaRepository.ObtenerActivasPorNinoAsync(persona.Id);
            var ninos = new List<Nino>();

            foreach (var autorizacion in autorizaciones)
            {
                var nino = await _ninoRepository.ObtenerPorIdAsync(autorizacion.NinoId);
                if (nino != null && nino.Activo)
                {
                    ninos.Add(nino);
                }
            }

            return ninos.OrderBy(n => n.Nombre).ToList();
        }

        public async Task<bool> PuedeRecogerNinoAsync(string cedula, int ninoId)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null || !nino.Activo)
            {
                return false;
            }

            return await _ninoPersonaRepository.PuedeRecogerNinoAsync(cedula, ninoId);
        }

        public async Task<PersonaAutorizada> ValidarYObtenerPersonaAutorizadaAsync(string cedula, int ninoId)
        {
            if (!await PuedeRecogerNinoAsync(cedula, ninoId))
            {
                throw new UnauthorizedAccessException("La persona no está autorizada para recoger a este niño");
            }

            var persona = await _personaAutorizadaRepository.ObtenerPorCedulaAsync(cedula);
            if (persona == null)
            {
                throw new ArgumentException("No se encontró una persona autorizada con la cédula especificada");
            }

            return persona;
        }

        public async Task RegistrarRecogidaAsync(int ninoId, string cedula, DateTime fechaHora)
        {
            var persona = await ValidarYObtenerPersonaAutorizadaAsync(cedula, ninoId);

            if (fechaHora.Date != DateTime.Now.Date)
            {
                throw new ArgumentException("Solo se pueden registrar recogidas del día actual");
            }

            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);

            // TODO: agregar logica adicional para registrar la recogida
            // Por ejemplo, crear una entidad RecogidaNino o agregar un log
            // ----
            // ----

            var autorizacion = await _ninoPersonaRepository.ObtenerRelacionAsync(ninoId, persona.Id);
            if (autorizacion != null)
            {
                var observacionAnterior = autorizacion.Observaciones ?? "";
                autorizacion.Observaciones = $"{observacionAnterior} | Recogida: {fechaHora:dd/MM/yyyy HH:mm}";
                await _ninoPersonaRepository.ActualizarAsync(autorizacion);
            }
        }

        public async Task<List<NinoPersonaAutorizada>> ObtenerHistorialAutorizacionesAsync(int ninoId)
        {
            return await _ninoPersonaRepository.ObtenerPorNinoIdAsync(ninoId);
        }

        public async Task<List<NinoPersonaAutorizada>> ObtenerAutorizacionesRevocadasAsync(int ninoId)
        {
            var todasLasAutorizaciones = await _ninoPersonaRepository.ObtenerPorNinoIdAsync(ninoId);
            return todasLasAutorizaciones.Where(a => !a.Activa && a.FechaRevocacion.HasValue).ToList();
        }
    }
}