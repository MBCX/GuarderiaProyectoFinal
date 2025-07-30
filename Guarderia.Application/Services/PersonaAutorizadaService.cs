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
    public class PersonaAutorizadaService : IPersonaAutorizadaService
    {
        private readonly IPersonaAutorizadaRepository _personaAutorizadaRepository;
        private readonly INinoPersonaAutorizadaRepository _ninoPersonaRepository;
        private readonly INinoRepository _ninoRepository;

        public PersonaAutorizadaService(
            IPersonaAutorizadaRepository personaAutorizadaRepository,
            INinoPersonaAutorizadaRepository ninoPersonaRepository,
            INinoRepository ninoRepository)
        {
            _personaAutorizadaRepository = personaAutorizadaRepository;
            _ninoPersonaRepository = ninoPersonaRepository;
            _ninoRepository = ninoRepository;
        }

        public async Task<List<PersonaAutorizada>> ObtenerPersonasAutorizadasPorNiñoAsync(int ninoId)
        {
            return await _personaAutorizadaRepository.ObtenerPorNinoIdAsync(ninoId);
        }

        public async Task<List<PersonaAutorizada>> ObtenerPersonasActivasPorNiñoAsync(int ninoId)
        {
            return await _personaAutorizadaRepository.ObtenerActivasPorNinoIdAsync(ninoId);
        }

        public async Task<PersonaAutorizada> ObtenerPorCedulaAsync(string cedula)
        {
            return await _personaAutorizadaRepository.ObtenerPorCedulaAsync(cedula);
        }

        public async Task<int> RegistrarPersonaAutorizadaAsync(PersonaAutorizada personaAutorizada)
        {
            // Validaciones
            if (!await ValidarDatosObligatoriosPersonaAsync(personaAutorizada))
            {
                throw new ArgumentException("Faltan datos obligatorios de la persona autorizada");
            }

            if (await _personaAutorizadaRepository.ExistePorCedulaAsync(personaAutorizada.Cedula))
            {
                throw new InvalidOperationException("Ya existe una persona autorizada con esa cédula");
            }

            // Limpiar y normalizar datos
            personaAutorizada.Cedula = personaAutorizada.Cedula.Trim();
            personaAutorizada.Nombre = personaAutorizada.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(personaAutorizada.Direccion))
            {
                personaAutorizada.Direccion = personaAutorizada.Direccion.Trim();
            }

            if (!string.IsNullOrWhiteSpace(personaAutorizada.Telefono))
            {
                personaAutorizada.Telefono = personaAutorizada.Telefono.Trim();
            }

            await _personaAutorizadaRepository.AgregarAsync(personaAutorizada);
            return personaAutorizada.Id;
        }

        public async Task ActualizarPersonaAutorizadaAsync(PersonaAutorizada personaAutorizada)
        {
            if (personaAutorizada == null)
            {
                throw new ArgumentException("La persona autorizada no puede ser nula");
            }

            var personaExistente = await _personaAutorizadaRepository.ObtenerPorIdAsync(personaAutorizada.Id);
            if (personaExistente == null)
            {
                throw new ArgumentException("La persona autorizada especificada no existe");
            }

            if (!await ValidarDatosObligatoriosPersonaAsync(personaAutorizada))
            {
                throw new ArgumentException("Faltan datos obligatorios de la persona autorizada");
            }

            // Verificar cedula unica si cambio
            if (personaExistente.Cedula != personaAutorizada.Cedula)
            {
                if (await _personaAutorizadaRepository.ExistePorCedulaAsync(personaAutorizada.Cedula))
                {
                    throw new InvalidOperationException("Ya existe una persona autorizada con esa cédula");
                }
            }

            // Limpiar datos
            personaAutorizada.Cedula = personaAutorizada.Cedula.Trim();
            personaAutorizada.Nombre = personaAutorizada.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(personaAutorizada.Direccion))
            {
                personaAutorizada.Direccion = personaAutorizada.Direccion.Trim();
            }

            if (!string.IsNullOrWhiteSpace(personaAutorizada.Telefono))
            {
                personaAutorizada.Telefono = personaAutorizada.Telefono.Trim();
            }

            await _personaAutorizadaRepository.ActualizarAsync(personaAutorizada);
        }

        public async Task AutorizarParaNiñoAsync(int ninoId, string cedula, string relacion)
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

            var persona = await _personaAutorizadaRepository.ObtenerPorCedulaAsync(cedula);
            if (persona == null)
            {
                throw new ArgumentException("La persona autorizada con esa cedula no existe. Debe registrarla primero.");
            }

            if (await _personaAutorizadaRepository.EstaAutorizadaParaNinoAsync(cedula, ninoId))
            {
                throw new InvalidOperationException("La persona ya está autorizada para recoger a este niño");
            }

            var autorizacion = new NinoPersonaAutorizada
            {
                NinoId = ninoId,
                PersonaAutorizadaId = persona.Id,
                FechaAutorizacion = DateTime.Now,
                Activa = true,
                Observaciones = $"Relación: {relacion}"
            };

            await _ninoPersonaRepository.AgregarAsync(autorizacion);
        }

        public async Task RevocarAutorizacionAsync(int ninoId, string cedula, DateTime fechaRevocacion)
        {
            var persona = await _personaAutorizadaRepository.ObtenerPorCedulaAsync(cedula);
            if (persona == null)
            {
                throw new ArgumentException("No se encontró una persona autorizada con esa cédula");
            }

            await _ninoPersonaRepository.RevocarAutorizacionAsync(ninoId, persona.Id, fechaRevocacion);
        }

        public async Task<bool> ValidarPersonaAutorizadaAsync(string cedula, int ninoId)
        {
            return await _personaAutorizadaRepository.EstaAutorizadaParaNinoAsync(cedula, ninoId);
        }

        public async Task<bool> ValidarDatosObligatoriosPersonaAsync(PersonaAutorizada persona)
        {
            if (persona == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(persona.Cedula))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(persona.Nombre))
            {
                return false;
            }

            // Validar formato de cedula básico (solo números y guiones)
            var cedulaLimpia = persona.Cedula.Replace("-", "").Replace(" ", "");
            if (cedulaLimpia.Length < 8 || !cedulaLimpia.All(char.IsDigit))
            {
                return false;
            }

            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> PuedeRecogerNiñoAsync(string cedula, int ninoId)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null || !nino.Activo)
            {
                return false;
            }

            return await _ninoPersonaRepository.PuedeRecogerNinoAsync(cedula, ninoId);
        }

        public async Task EliminarPersonaAutorizadaAsync(int id)
        {
            var persona = await _personaAutorizadaRepository.ObtenerPorIdAsync(id);
            if (persona == null)
            {
                throw new ArgumentException("La persona autorizada especificada no existe");
            }

            var autorizacionesActivas = await _ninoPersonaRepository.ObtenerPorPersonaIdAsync(id);
            var tieneAutorizacionesActivas = autorizacionesActivas.Any(a => a.Activa);

            if (tieneAutorizacionesActivas)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar la persona porque tiene autorizaciones activas. " +
                    "Debe revocar todas las autorizaciones primero.");
            }

            await _personaAutorizadaRepository.EliminarAsync(id);
        }
    }
}