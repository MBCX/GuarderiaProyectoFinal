using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Main.Menu.Ingrediente;
using Guarderia.Domain.Entities;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientesController : ControllerBase
    {
        private readonly IIngredienteService _ingredienteService;
        private readonly ILogger<IngredientesController> _logger;

        public IngredientesController(
            IIngredienteService ingredienteService,
            ILogger<IngredientesController> logger)
        {
            _ingredienteService = ingredienteService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los ingredientes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<IngredienteDto>>> GetTodos()
        {
            try
            {
                var ingredientes = await _ingredienteService.ObtenerTodosAsync();
                var ingredientesDto = ingredientes.Select(i => MapToDto(i)).ToList();
                return Ok(ingredientesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los ingredientes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un ingrediente por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<IngredienteDto>> GetById(int id)
        {
            try
            {
                var ingrediente = await _ingredienteService.ObtenerPorIdAsync(id);
                if (ingrediente == null)
                {
                    return NotFound($"No se encontró el ingrediente con ID {id}");
                }

                var ingredienteDto = MapToDto(ingrediente);
                return Ok(ingredienteDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingrediente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un ingrediente por su nombre
        /// </summary>
        [HttpGet("nombre/{nombre}")]
        public async Task<ActionResult<IngredienteDto>> GetByNombre(string nombre)
        {
            try
            {
                var ingrediente = await _ingredienteService.ObtenerPorNombreAsync(nombre);
                if (ingrediente == null)
                {
                    return NotFound($"No se encontró el ingrediente con nombre {nombre}");
                }

                var ingredienteDto = MapToDto(ingrediente);
                return Ok(ingredienteDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingrediente con nombre {Nombre}", nombre);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los ingredientes que son alérgenos
        /// </summary>
        [HttpGet("alergenos")]
        public async Task<ActionResult<List<IngredienteDto>>> GetAlergenos()
        {
            try
            {
                var ingredientes = await _ingredienteService.ObtenerAlergenosAsync();
                var ingredientesDto = ingredientes.Select(i => MapToDto(i)).ToList();
                return Ok(ingredientesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingredientes alérgenos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los ingredientes que no son alérgenos
        /// </summary>
        [HttpGet("no-alergenos")]
        public async Task<ActionResult<List<IngredienteDto>>> GetNoAlergenos()
        {
            try
            {
                var ingredientes = await _ingredienteService.ObtenerNoAlergenosAsync();
                var ingredientesDto = ingredientes.Select(i => MapToDto(i)).ToList();
                return Ok(ingredientesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingredientes no alérgenos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo ingrediente
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<IngredienteDto>> Create([FromBody] CrearIngredienteDto crearIngredienteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var ingrediente = new Ingrediente
                {
                    Nombre = crearIngredienteDto.Nombre,
                    Descripcion = crearIngredienteDto.Descripcion ?? string.Empty,
                    EsAlergeno = crearIngredienteDto.EsAlergeno
                };

                var ingredienteId = await _ingredienteService.CrearIngredienteAsync(ingrediente);
                var ingredienteCreado = await _ingredienteService.ObtenerPorIdAsync(ingredienteId);

                var ingredienteDto = MapToDto(ingredienteCreado);
                return CreatedAtAction(nameof(GetById), new { id = ingredienteId }, ingredienteDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear ingrediente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un ingrediente existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<IngredienteDto>> Update(int id, [FromBody] IngredienteDto ingredienteDto)
        {
            try
            {
                if (id != ingredienteDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var ingrediente = new Ingrediente
                {
                    Id = ingredienteDto.Id,
                    Nombre = ingredienteDto.Nombre,
                    Descripcion = ingredienteDto.Descripcion ?? string.Empty,
                    EsAlergeno = ingredienteDto.EsAlergeno
                };

                await _ingredienteService.ActualizarIngredienteAsync(ingrediente);

                var ingredienteActualizado = await _ingredienteService.ObtenerPorIdAsync(id);
                var ingredienteActualizadoDto = MapToDto(ingredienteActualizado);
                return Ok(ingredienteActualizadoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar ingrediente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Marca un ingrediente como alérgeno
        /// </summary>
        [HttpPatch("{id}/marcar-alergeno")]
        public async Task<ActionResult> MarcarComoAlergeno(int id)
        {
            try
            {
                var ingrediente = await _ingredienteService.ObtenerPorIdAsync(id);
                if (ingrediente == null)
                {
                    return NotFound($"No se encontró el ingrediente con ID {id}");
                }

                await _ingredienteService.MarcarComoAlergenoAsync(id);
                return Ok(new { mensaje = "Ingrediente marcado como alérgeno exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar ingrediente como alérgeno con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Desmarca un ingrediente como alérgeno
        /// </summary>
        [HttpPatch("{id}/desmarcar-alergeno")]
        public async Task<ActionResult> DesmarcarComoAlergeno(int id)
        {
            try
            {
                var ingrediente = await _ingredienteService.ObtenerPorIdAsync(id);
                if (ingrediente == null)
                {
                    return NotFound($"No se encontró el ingrediente con ID {id}");
                }

                await _ingredienteService.DesmarcarComoAlergenoAsync(id);
                return Ok(new { mensaje = "Ingrediente desmarcado como alérgeno exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desmarcar ingrediente como alérgeno con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un ingrediente existe por nombre
        /// </summary>
        [HttpGet("existe/{nombre}")]
        public async Task<ActionResult<bool>> Existe(string nombre)
        {
            try
            {
                var existe = await _ingredienteService.ExisteAsync(nombre);
                return Ok(existe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si existe el ingrediente {Nombre}", nombre);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un ingrediente es alérgeno
        /// </summary>
        [HttpGet("{id}/es-alergeno")]
        public async Task<ActionResult<bool>> EsAlergeno(int id)
        {
            try
            {
                var esAlergeno = await _ingredienteService.EsAlergenoAsync(id);
                return Ok(esAlergeno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si el ingrediente {Id} es alérgeno", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los platos que contienen un ingrediente específico
        /// </summary>
        [HttpGet("{id}/platos")]
        public async Task<ActionResult<List<object>>> GetPlatosQueContienen(int id)
        {
            try
            {
                var ingrediente = await _ingredienteService.ObtenerPorIdAsync(id);
                if (ingrediente == null)
                {
                    return NotFound($"No se encontró el ingrediente con ID {id}");
                }

                var platos = await _ingredienteService.ObtenerPlatosQueContienenAsync(id);
                var platosDto = platos.Select(p => new { p.Id, p.Nombre, p.Descripcion }).ToList();
                return Ok(platosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener platos que contienen el ingrediente {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un ingrediente del sistema
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var ingrediente = await _ingredienteService.ObtenerPorIdAsync(id);
                if (ingrediente == null)
                {
                    return NotFound($"No se encontró el ingrediente con ID {id}");
                }

                await _ingredienteService.EliminarIngredienteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ingrediente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // Método auxiliar para mapear entidad a DTO
        private static IngredienteDto MapToDto(Ingrediente ingrediente)
        {
            return new IngredienteDto
            {
                Id = ingrediente.Id,
                Nombre = ingrediente.Nombre,
                Descripcion = ingrediente.Descripcion,
                EsAlergeno = ingrediente.EsAlergeno
            };
        }
    }
}