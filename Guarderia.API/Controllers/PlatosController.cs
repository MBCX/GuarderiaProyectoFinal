using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Main.Menu.Plato;
using Guarderia.Shared.DTO.Main.Menu.Ingrediente;
using Guarderia.Domain.Entities;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatosController : ControllerBase
    {
        private readonly IPlatoService _platoService;
        private readonly ILogger<PlatosController> _logger;

        public PlatosController(IPlatoService platoService, ILogger<PlatosController> logger)
        {
            _platoService = platoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los platos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PlatoDto>>> GetTodos()
        {
            try
            {
                var platos = await _platoService.ObtenerTodosAsync();
                var platosDto = await Task.WhenAll(platos.Select(async p => await MapToDtoAsync(p)));
                return Ok(platosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los platos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un plato por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PlatoDto>> GetById(int id)
        {
            try
            {
                var plato = await _platoService.ObtenerPorIdAsync(id);
                if (plato == null)
                {
                    return NotFound($"No se encontró el plato con ID {id}");
                }

                var platoDto = await MapToDtoAsync(plato);
                return Ok(platoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener plato con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un plato por su nombre
        /// </summary>
        [HttpGet("nombre/{nombre}")]
        public async Task<ActionResult<PlatoDto>> GetByNombre(string nombre)
        {
            try
            {
                var plato = await _platoService.ObtenerPorNombreAsync(nombre);
                if (plato == null)
                {
                    return NotFound($"No se encontró el plato con nombre {nombre}");
                }

                var platoDto = await MapToDtoAsync(plato);
                return Ok(platoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener plato con nombre {Nombre}", nombre);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los platos de un menú específico
        /// </summary>
        [HttpGet("menu/{menuId}")]
        public async Task<ActionResult<List<PlatoDto>>> GetPorMenu(int menuId)
        {
            try
            {
                var platos = await _platoService.ObtenerPorMenuAsync(menuId);
                var platosDto = await Task.WhenAll(platos.Select(async p => await MapToDtoAsync(p)));
                return Ok(platosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener platos del menú {MenuId}", menuId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo plato
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PlatoDto>> Create([FromBody] CrearPlatoDto crearPlatoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var plato = new Plato
                {
                    Nombre = crearPlatoDto.Nombre,
                    Descripcion = crearPlatoDto.Descripcion ?? string.Empty,
                    TipoPlato = crearPlatoDto.TipoPlato ?? string.Empty
                };

                var platoId = await _platoService.CrearPlatoAsync(plato, crearPlatoDto.IngredienteIds);
                var platoCreado = await _platoService.ObtenerPorIdAsync(platoId);

                var platoDto = await MapToDtoAsync(platoCreado);
                return CreatedAtAction(nameof(GetById), new { id = platoId }, platoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear plato");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un plato existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PlatoDto>> Update(int id, [FromBody] PlatoDto platoDto)
        {
            try
            {
                if (id != platoDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var plato = new Plato
                {
                    Id = platoDto.Id,
                    Nombre = platoDto.Nombre,
                    Descripcion = platoDto.Descripcion ?? string.Empty,
                    TipoPlato = platoDto.TipoPlato ?? string.Empty
                };

                await _platoService.ActualizarPlatoAsync(plato);

                var platoActualizado = await _platoService.ObtenerPorIdAsync(id);
                var platoActualizadoDto = await MapToDtoAsync(platoActualizado);
                return Ok(platoActualizadoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar plato con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Agrega un ingrediente a un plato
        /// </summary>
        [HttpPost("{platoId}/ingredientes/{ingredienteId}")]
        public async Task<ActionResult> AgregarIngrediente(int platoId, int ingredienteId, [FromBody] AgregarIngredienteDto agregarDto)
        {
            try
            {
                var plato = await _platoService.ObtenerPorIdAsync(platoId);
                if (plato == null)
                {
                    return NotFound($"No se encontró el plato con ID {platoId}");
                }

                await _platoService.AgregarIngredienteAsync(platoId, ingredienteId, agregarDto.Cantidad);
                return Ok(new { mensaje = "Ingrediente agregado exitosamente al plato" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar ingrediente {IngredienteId} al plato {PlatoId}", ingredienteId, platoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Remueve un ingrediente de un plato
        /// </summary>
        [HttpDelete("{platoId}/ingredientes/{ingredienteId}")]
        public async Task<ActionResult> RemoverIngrediente(int platoId, int ingredienteId)
        {
            try
            {
                var plato = await _platoService.ObtenerPorIdAsync(platoId);
                if (plato == null)
                {
                    return NotFound($"No se encontró el plato con ID {platoId}");
                }

                await _platoService.RemoverIngredienteAsync(platoId, ingredienteId);
                return Ok(new { mensaje = "Ingrediente removido exitosamente del plato" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover ingrediente {IngredienteId} del plato {PlatoId}", ingredienteId, platoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los ingredientes de un plato
        /// </summary>
        [HttpGet("{id}/ingredientes")]
        public async Task<ActionResult<List<IngredienteDto>>> GetIngredientes(int id)
        {
            try
            {
                var plato = await _platoService.ObtenerPorIdAsync(id);
                if (plato == null)
                {
                    return NotFound($"No se encontró el plato con ID {id}");
                }

                var ingredientes = await _platoService.ObtenerIngredientesAsync(id);
                var ingredientesDto = ingredientes.Select(i => new IngredienteDto
                {
                    Id = i.Id,
                    Nombre = i.Nombre,
                    Descripcion = i.Descripcion,
                    EsAlergeno = i.EsAlergeno
                }).ToList();

                return Ok(ingredientesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingredientes del plato {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un plato contiene un ingrediente específico
        /// </summary>
        [HttpGet("{id}/contiene-ingrediente/{nombreIngrediente}")]
        public async Task<ActionResult<bool>> ContieneIngrediente(int id, string nombreIngrediente)
        {
            try
            {
                var contiene = await _platoService.ContieneIngredienteAsync(id, nombreIngrediente);
                return Ok(contiene);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si el plato {Id} contiene el ingrediente {Ingrediente}", id, nombreIngrediente);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un plato es apto para un niño (sin alergias)
        /// </summary>
        [HttpGet("{id}/es-apto-para-nino/{ninoId}")]
        public async Task<ActionResult<bool>> EsAptoParaNino(int id, int ninoId)
        {
            try
            {
                var esApto = await _platoService.EsAptoParaNiñoAsync(id, ninoId);
                return Ok(esApto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si el plato {PlatoId} es apto para el niño {NinoId}", id, ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los alérgenos de un plato para un niño específico
        /// </summary>
        [HttpGet("{id}/alergenos-para-nino/{ninoId}")]
        public async Task<ActionResult<List<string>>> GetAlergenosEnPlato(int id, int ninoId)
        {
            try
            {
                var plato = await _platoService.ObtenerPorIdAsync(id);
                if (plato == null)
                {
                    return NotFound($"No se encontró el plato con ID {id}");
                }

                var alergenos = await _platoService.ObtenerAlergenosEnPlatoAsync(id, ninoId);
                return Ok(alergenos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener alérgenos del plato {PlatoId} para el niño {NinoId}", id, ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un plato del sistema
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var plato = await _platoService.ObtenerPorIdAsync(id);
                if (plato == null)
                {
                    return NotFound($"No se encontró el plato con ID {id}");
                }

                await _platoService.EliminarPlatoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar plato con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // Método auxiliar para mapear entidad a DTO
        private async Task<PlatoDto> MapToDtoAsync(Plato plato)
        {
            var ingredientes = await _platoService.ObtenerIngredientesAsync(plato.Id);

            return new PlatoDto
            {
                Id = plato.Id,
                Nombre = plato.Nombre,
                Descripcion = plato.Descripcion,
                TipoPlato = plato.TipoPlato,
                Ingredientes = ingredientes.Select(i => new IngredienteDto
                {
                    Id = i.Id,
                    Nombre = i.Nombre,
                    Descripcion = i.Descripcion,
                    EsAlergeno = i.EsAlergeno
                }).ToList(),
                NombresIngredientes = ingredientes.Select(i => i.Nombre).ToList()
            };
        }
    }

    // DTO auxiliar para agregar ingredientes
    public class AgregarIngredienteDto
    {
        public string Cantidad { get; set; } = string.Empty;
    }
}