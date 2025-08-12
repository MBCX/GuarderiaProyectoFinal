using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Main.PersonaAutorizada;
using Guarderia.Domain.Entities;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasAutorizadasController : ControllerBase
    {
        private readonly IPersonaAutorizadaService _personaAutorizadaService;
        private readonly ILogger<PersonasAutorizadasController> _logger;

        public PersonasAutorizadasController(
            IPersonaAutorizadaService personaAutorizadaService,
            ILogger<PersonasAutorizadasController> logger
        )
        {
            _personaAutorizadaService = personaAutorizadaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene las personas autorizadas para recoger a un niño específico
        /// </summary>
        [HttpGet("nino/{ninoId}")]
        public async Task<ActionResult<List<PersonaAutorizadaDto>>> GetPorNino(int ninoId)
        {
            try
            {
                var personas = await _personaAutorizadaService.ObtenerPersonasAutorizadasPorNiñoAsync(ninoId);
                var personasDto = personas.Select(p => MapToDto(p)).ToList();
                return Ok(personasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personas autorizadas para el niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las personas activamente autorizadas para recoger a un niño específico
        /// </summary>
        [HttpGet("nino/{ninoId}/activas")]
        public async Task<ActionResult<List<PersonaAutorizadaDto>>> GetActivasPorNino(int ninoId)
        {
            try
            {
                var personas = await _personaAutorizadaService.ObtenerPersonasActivasPorNiñoAsync(ninoId);
                var personasDto = personas.Select(p => MapToDto(p)).ToList();
                return Ok(personasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personas activas autorizadas para el niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una persona autorizada por su cédula
        /// </summary>
        [HttpGet("cedula/{cedula}")]
        public async Task<ActionResult<PersonaAutorizadaDto>> GetPorCedula(string cedula)
        {
            try
            {
                var persona = await _personaAutorizadaService.ObtenerPorCedulaAsync(cedula);
                if (persona == null)
                {
                    return NotFound($"No se encontró persona autorizada con cédula {cedula}");
                }

                var personaDto = MapToDto(persona);
                return Ok(personaDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener persona autorizada con cédula {Cedula}", cedula);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Registra una nueva persona autorizada
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PersonaAutorizadaDto>> Create([FromBody] CrearPersonaAutorizadaDto crearPersonaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var persona = new PersonaAutorizada
                {
                    Cedula = crearPersonaDto.Cedula,
                    Nombre = crearPersonaDto.Nombre,
                    Direccion = crearPersonaDto.Direccion ?? string.Empty,
                    Telefono = crearPersonaDto.Telefono ?? string.Empty,
                    Relacion = crearPersonaDto.Relacion ?? string.Empty
                };

                var personaId = await _personaAutorizadaService.RegistrarPersonaAutorizadaAsync(persona);
                var personaCreada = await _personaAutorizadaService.ObtenerPorCedulaAsync(persona.Cedula);

                var personaDto = MapToDto(personaCreada);
                return CreatedAtAction(nameof(GetPorCedula), new { cedula = persona.Cedula }, personaDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear persona autorizada");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza los datos de una persona autorizada
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PersonaAutorizadaDto>> Update(int id, [FromBody] PersonaAutorizadaDto personaDto)
        {
            try
            {
                if (id != personaDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var persona = new PersonaAutorizada
                {
                    Id = personaDto.Id,
                    Cedula = personaDto.Cedula,
                    Nombre = personaDto.Nombre,
                    Direccion = personaDto.Direccion ?? string.Empty,
                    Telefono = personaDto.Telefono ?? string.Empty,
                    Relacion = personaDto.Relacion ?? string.Empty
                };

                await _personaAutorizadaService.ActualizarPersonaAutorizadaAsync(persona);

                var personaActualizada = await _personaAutorizadaService.ObtenerPorCedulaAsync(persona.Cedula);
                var personaActualizadaDto = MapToDto(personaActualizada);
                return Ok(personaActualizadaDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar persona autorizada con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Autoriza a una persona para recoger a un niño específico
        /// </summary>
        [HttpPost("autorizar")]
        public async Task<ActionResult> AutorizarParaNino([FromBody] AutorizarPersonaDto autorizarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _personaAutorizadaService.AutorizarParaNiñoAsync(
                    autorizarDto.NinoId,
                    autorizarDto.Cedula,
                    autorizarDto.Relacion);

                return Ok(new { mensaje = "Persona autorizada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al autorizar persona");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Revoca la autorización de una persona para recoger a un niño
        /// </summary>
        [HttpPost("revocar")]
        public async Task<ActionResult> RevocarAutorizacion([FromBody] RevocarAutorizacionDto revocarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _personaAutorizadaService.RevocarAutorizacionAsync(
                    revocarDto.NinoId,
                    revocarDto.Cedula,
                    revocarDto.FechaRevocacion);

                return Ok(new { mensaje = "Autorización revocada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revocar autorización");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida si una persona está autorizada para recoger a un niño específico
        /// </summary>
        [HttpGet("validar/{cedula}/nino/{ninoId}")]
        public async Task<ActionResult<bool>> ValidarAutorizacion(string cedula, int ninoId)
        {
            try
            {
                var esValida = await _personaAutorizadaService.ValidarPersonaAutorizadaAsync(cedula, ninoId);
                return Ok(esValida);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar autorización para cédula {Cedula} y niño {NinoId}", cedula, ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si una persona puede recoger a un niño específico
        /// </summary>
        [HttpGet("puede-recoger/{cedula}/nino/{ninoId}")]
        public async Task<ActionResult<bool>> PuedeRecogerNino(string cedula, int ninoId)
        {
            try
            {
                var puedeRecoger = await _personaAutorizadaService.PuedeRecogerNiñoAsync(cedula, ninoId);
                return Ok(puedeRecoger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si puede recoger niño para cédula {Cedula} y niño {NinoId}", cedula, ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una persona autorizada del sistema
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _personaAutorizadaService.EliminarPersonaAutorizadaAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar persona autorizada con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // Método auxiliar para mapear entidad a DTO
        private static PersonaAutorizadaDto MapToDto(PersonaAutorizada persona)
        {
            return new PersonaAutorizadaDto
            {
                Id = persona.Id,
                Cedula = persona.Cedula,
                Nombre = persona.Nombre,
                Direccion = persona.Direccion,
                Telefono = persona.Telefono,
                Relacion = persona.Relacion
            };
        }
    }

    // DTOs adicionales para operaciones específicas
    public class RevocarAutorizacionDto
    {
        public int NinoId { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public DateTime FechaRevocacion { get; set; }
    }
}