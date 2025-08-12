using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Main.Nino;
using Guarderia.Domain.Entities;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NinosController : ControllerBase
    {
        private readonly INinoService _ninoService;
        private readonly ILogger<NinosController> _logger;

        public NinosController(INinoService ninoService, ILogger<NinosController> logger)
        {
            _ninoService = ninoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los niños registrados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<NinoDto>>> GetTodos()
        {
            try
            {
                var ninos = await _ninoService.ObtenerTodosLosNinosAsync();
                var ninosDto = ninos.Select(n => MapToDto(n)).ToList();
                return Ok(ninosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los niños");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los niños activos
        /// </summary>
        [HttpGet("activos")]
        public async Task<ActionResult<List<NinoDto>>> GetActivos()
        {
            try
            {
                var ninos = await _ninoService.ObtenerNinosActivosAsync();
                var ninosDto = ninos.Select(n => MapToDto(n)).ToList();
                return Ok(ninosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener niños activos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los niños inactivos
        /// </summary>
        [HttpGet("inactivos")]
        public async Task<ActionResult<List<NinoDto>>> GetInactivos()
        {
            try
            {
                var ninos = await _ninoService.ObtenerNinosInactivosAsync();
                var ninosDto = ninos.Select(n => MapToDto(n)).ToList();
                return Ok(ninosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener niños inactivos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un niño por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<NinoDto>> GetById(int id)
        {
            try
            {
                var nino = await _ninoService.ObtenerNinoPorIdAsync(id);
                if (nino == null)
                {
                    return NotFound($"No se encontró el niño con ID {id}");
                }

                var ninoDto = MapToDto(nino);
                return Ok(ninoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener niño con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un niño por su número de matrícula
        /// </summary>
        [HttpGet("matricula/{numeroMatricula}")]
        public async Task<ActionResult<NinoDto>> GetByMatricula(string numeroMatricula)
        {
            try
            {
                var nino = await _ninoService.ObtenerPorMatriculaAsync(numeroMatricula);
                if (nino == null)
                {
                    return NotFound($"No se encontró el niño con matrícula {numeroMatricula}");
                }

                var ninoDto = MapToDto(nino);
                return Ok(ninoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener niño con matrícula {Matricula}", numeroMatricula);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los niños de un responsable de pago
        /// </summary>
        [HttpGet("responsable/{responsablePagoId}")]
        public async Task<ActionResult<List<NinoDto>>> GetByResponsablePago(int responsablePagoId)
        {
            try
            {
                var ninos = await _ninoService.ObtenerPorResponsablePagoAsync(responsablePagoId);
                var ninosDto = ninos.Select(n => MapToDto(n)).ToList();
                return Ok(ninosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener niños del responsable {ResponsableId}", responsablePagoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Registra un nuevo niño
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<NinoDto>> Create([FromBody] CrearNinoDto crearNinoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var nino = new Nino
                {
                    Nombre = crearNinoDto.Nombre,
                    FechaNacimiento = crearNinoDto.FechaNacimiento,
                    NumeroMatricula = crearNinoDto.NumeroMatricula ?? await _ninoService.GenerarNumeroMatriculaAsync(),
                    FechaIngreso = DateTime.Now,
                    Activo = true,
                    ResponsablePagoId = crearNinoDto.ResponsablePagoId
                };

                var ninoId = await _ninoService.RegistrarNinoAsync(nino, crearNinoDto.ResponsablePagoId);
                var ninoCreado = await _ninoService.ObtenerNinoPorIdAsync(ninoId);

                var ninoDto = MapToDto(ninoCreado);
                return CreatedAtAction(nameof(GetById), new { id = ninoId }, ninoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear niño");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza los datos de un niño
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<NinoDto>> Update(int id, [FromBody] ActualizarNinoDto actualizarNinoDto)
        {
            try
            {
                if (id != actualizarNinoDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var ninoExistente = await _ninoService.ObtenerNinoPorIdAsync(id);
                if (ninoExistente == null)
                {
                    return NotFound($"No se encontró el niño con ID {id}");
                }

                ninoExistente.Nombre = actualizarNinoDto.Nombre;
                ninoExistente.FechaNacimiento = actualizarNinoDto.FechaNacimiento;
                ninoExistente.NumeroMatricula = actualizarNinoDto.NumeroMatricula;
                ninoExistente.ResponsablePagoId = actualizarNinoDto.ResponsablePagoId;

                await _ninoService.ActualizarNinoAsync(ninoExistente);

                var ninoActualizado = await _ninoService.ObtenerNinoPorIdAsync(id);
                var ninoDto = MapToDto(ninoActualizado);
                return Ok(ninoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar niño con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Da de baja a un niño
        /// </summary>
        [HttpPatch("{id}/baja")]
        public async Task<ActionResult> DarBaja(int id, [FromBody] DateTime fechaBaja)
        {
            try
            {
                var nino = await _ninoService.ObtenerNinoPorIdAsync(id);
                if (nino == null)
                {
                    return NotFound($"No se encontró el niño con ID {id}");
                }

                await _ninoService.DarBajaNinoAsync(id, fechaBaja);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar de baja al niño con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Reactiva un niño que estaba dado de baja
        /// </summary>
        [HttpPatch("{id}/reactivar")]
        public async Task<ActionResult> Reactivar(int id)
        {
            try
            {
                var nino = await _ninoService.ObtenerNinoPorIdAsync(id);
                if (nino == null)
                {
                    return NotFound($"No se encontró el niño con ID {id}");
                }

                await _ninoService.ReactivarNinoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reactivar al niño con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un niño del sistema
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var nino = await _ninoService.ObtenerNinoPorIdAsync(id);
                if (nino == null)
                {
                    return NotFound($"No se encontró el niño con ID {id}");
                }

                await _ninoService.EliminarNinoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar niño con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las alergias de un niño
        /// </summary>
        [HttpGet("{id}/alergias")]
        public async Task<ActionResult<List<string>>> GetAlergias(int id)
        {
            try
            {
                var nino = await _ninoService.ObtenerNinoPorIdAsync(id);
                if (nino == null)
                {
                    return NotFound($"No se encontró el niño con ID {id}");
                }

                var alergias = await _ninoService.ObtenerAlergiasAsync(id);
                return Ok(alergias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener alergias del niño con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si existe una matrícula
        /// </summary>
        [HttpGet("verificar-matricula/{numeroMatricula}")]
        public async Task<ActionResult<bool>> VerificarMatricula(string numeroMatricula)
        {
            try
            {
                var existe = await _ninoService.ExisteMatriculaAsync(numeroMatricula);
                return Ok(existe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar matrícula {Matricula}", numeroMatricula);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Genera un nuevo número de matrícula
        /// </summary>
        [HttpGet("generar-matricula")]
        public async Task<ActionResult<string>> GenerarMatricula()
        {
            try
            {
                var numeroMatricula = await _ninoService.GenerarNumeroMatriculaAsync();
                return Ok(numeroMatricula);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar número de matrícula");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el conteo de niños activos
        /// </summary>
        [HttpGet("conteo/activos")]
        public async Task<ActionResult<int>> GetConteoActivos()
        {
            try
            {
                var conteo = await _ninoService.ContarNinosActivosAsync();
                return Ok(conteo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conteo de niños activos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // Método auxiliar para mapear entidad a DTO
        private static NinoDto MapToDto(Nino nino)
        {
            return new NinoDto
            {
                Id = nino.Id,
                NumeroMatricula = nino.NumeroMatricula,
                Nombre = nino.Nombre,
                FechaNacimiento = nino.FechaNacimiento,
                FechaIngreso = nino.FechaIngreso,
                FechaBaja = nino.FechaBaja,
                Activo = nino.Activo,
                ResponsablePagoId = nino.ResponsablePagoId
            };
        }
    }
}