using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Main.Alergia;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlergiasController : ControllerBase
    {
        private readonly IAlergiaService _alergiaService;
        private readonly ILogger<AlergiasController> _logger;

        public AlergiasController(IAlergiaService alergiaService, ILogger<AlergiasController> logger)
        {
            _alergiaService = alergiaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene las alergias de un niño específico
        /// </summary>
        [HttpGet("nino/{ninoId}")]
        public async Task<ActionResult<List<string>>> GetAlergiasPorNino(int ninoId)
        {
            try
            {
                var alergias = await _alergiaService.ObtenerAlergiasPorNiñoAsync(ninoId);
                return Ok(alergias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener alergias del niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las alergias completas de un niño específico (con detalles)
        /// </summary>
        [HttpGet("nino/{ninoId}/completas")]
        public async Task<ActionResult<List<AlergiaDto>>> GetAlergiasCompletasPorNino(int ninoId)
        {
            try
            {
                var alergias = await _alergiaService.ObtenerAlergiasCompletasPorNiñoAsync(ninoId);
                var alergiasDto = alergias.Select(a => new AlergiaDto
                {
                    Id = a.Id,
                    NinoId = ninoId,
                    IngredienteId = a.Ingrediente?.Id ?? 0,
                    IngredienteNombre = a.Ingrediente?.Nombre ?? string.Empty
                }).ToList();

                return Ok(alergiasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener alergias completas del niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Registra una nueva alergia para un niño
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> RegistrarAlergia([FromBody] RegistrarAlergiaDto registrarAlergiaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _alergiaService.RegistrarAlergiaAsync(registrarAlergiaDto.NinoId, registrarAlergiaDto.Ingrediente);
                return Ok(new { mensaje = "Alergia registrada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar alergia");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Registra una nueva alergia por ID de ingrediente
        /// </summary>
        [HttpPost("por-ingrediente-id")]
        public async Task<ActionResult> RegistrarAlergiaPorIngredienteId([FromBody] RegistrarAlergiaPorIdDto registrarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _alergiaService.RegistrarAlergiaAsync(registrarDto.NinoId, registrarDto.IngredienteId);
                return Ok(new { mensaje = "Alergia registrada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar alergia por ID de ingrediente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una alergia de un niño por nombre de ingrediente
        /// </summary>
        [HttpDelete("nino/{ninoId}/ingrediente/{ingrediente}")]
        public async Task<ActionResult> EliminarAlergia(int ninoId, string ingrediente)
        {
            try
            {
                await _alergiaService.EliminarAlergiaAsync(ninoId, ingrediente);
                return Ok(new { mensaje = "Alergia eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar alergia del niño {NinoId} al ingrediente {Ingrediente}", ninoId, ingrediente);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una alergia de un niño por ID de ingrediente
        /// </summary>
        [HttpDelete("nino/{ninoId}/ingrediente-id/{ingredienteId}")]
        public async Task<ActionResult> EliminarAlergiaPorId(int ninoId, int ingredienteId)
        {
            try
            {
                await _alergiaService.EliminarAlergiaAsync(ninoId, ingredienteId);
                return Ok(new { mensaje = "Alergia eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar alergia del niño {NinoId} al ingrediente ID {IngredienteId}", ninoId, ingredienteId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza todas las alergias de un niño
        /// </summary>
        [HttpPut("nino/{ninoId}")]
        public async Task<ActionResult> ActualizarAlergias(int ninoId, [FromBody] ActualizarAlergiaDto actualizarDto)
        {
            try
            {
                if (ninoId != actualizarDto.NinoId)
                {
                    return BadRequest("El ID del niño en la URL no coincide con el ID en el objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _alergiaService.ActualizarAlergiasAsync(ninoId, actualizarDto.NuevosIngredientesAlergicos);
                return Ok(new { mensaje = "Alergias actualizadas exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar alergias del niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza alergias por IDs de ingredientes
        /// </summary>
        [HttpPut("nino/{ninoId}/por-ids")]
        public async Task<ActionResult> ActualizarAlergiasPorIds(int ninoId, [FromBody] ActualizarAlergiaPorIdsDto actualizarDto)
        {
            try
            {
                if (ninoId != actualizarDto.NinoId)
                {
                    return BadRequest("El ID del niño en la URL no coincide con el ID en el objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _alergiaService.ActualizarAlergiasAsync(ninoId, actualizarDto.NuevosIngredienteIds);
                return Ok(new { mensaje = "Alergias actualizadas exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar alergias por IDs del niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un niño tiene alergia a un ingrediente específico
        /// </summary>
        [HttpGet("nino/{ninoId}/tiene-alergia/{ingrediente}")]
        public async Task<ActionResult<bool>> TieneAlergia(int ninoId, string ingrediente)
        {
            try
            {
                var tieneAlergia = await _alergiaService.TieneAlergiaAIngredienteAsync(ninoId, ingrediente);
                return Ok(tieneAlergia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar alergia del niño {NinoId} al ingrediente {Ingrediente}", ninoId, ingrediente);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un niño tiene alergia a un ingrediente por ID
        /// </summary>
        [HttpGet("nino/{ninoId}/tiene-alergia-id/{ingredienteId}")]
        public async Task<ActionResult<bool>> TieneAlergiaPorId(int ninoId, int ingredienteId)
        {
            try
            {
                var tieneAlergia = await _alergiaService.TieneAlergiaAIngredienteAsync(ninoId, ingredienteId);
                return Ok(tieneAlergia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar alergia del niño {NinoId} al ingrediente ID {IngredienteId}", ninoId, ingredienteId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un niño puede consumir un plato específico
        /// </summary>
        [HttpGet("nino/{ninoId}/puede-consumir-plato/{platoId}")]
        public async Task<ActionResult<bool>> PuedeConsumirPlato(int ninoId, int platoId)
        {
            try
            {
                var puedeConsumir = await _alergiaService.PuedeConsumirPlatoAsync(ninoId, platoId);
                return Ok(puedeConsumir);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si el niño {NinoId} puede consumir el plato {PlatoId}", ninoId, platoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un niño puede consumir un menú específico
        /// </summary>
        [HttpGet("nino/{ninoId}/puede-consumir-menu/{menuId}")]
        public async Task<ActionResult<bool>> PuedeConsumirMenu(int ninoId, int menuId)
        {
            try
            {
                var puedeConsumir = await _alergiaService.PuedeConsumirMenuAsync(ninoId, menuId);
                return Ok(puedeConsumir);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si el niño {NinoId} puede consumir el menú {MenuId}", ninoId, menuId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida un menú contra las alergias de un niño
        /// </summary>
        [HttpGet("nino/{ninoId}/validar-menu/{menuId}")]
        public async Task<ActionResult<List<string>>> ValidarMenuContraAlergias(int ninoId, int menuId)
        {
            try
            {
                var alergiasEncontradas = await _alergiaService.ValidarMenuContraAlergiasAsync(ninoId, menuId);
                return Ok(alergiasEncontradas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar menú {MenuId} contra alergias del niño {NinoId}", menuId, ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los niños que tienen alergias registradas
        /// </summary>
        [HttpGet("ninos-con-alergias")]
        public async Task<ActionResult<List<object>>> GetNinosConAlergias()
        {
            try
            {
                var ninos = await _alergiaService.ObtenerNiñosConAlergiasAsync();
                var ninosDto = ninos.Select(n => new
                {
                    n.Id,
                    n.Nombre,
                    n.NumeroMatricula,
                    CantidadAlergias = n.Alergias?.Count ?? 0,
                    Alergias = n.Alergias?.Select(a => a.Ingrediente?.Nombre).ToList() ?? new List<string>()
                }).ToList();

                return Ok(ninosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener niños con alergias");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene la lista de ingredientes alérgenos registrados
        /// </summary>
        [HttpGet("ingredientes-alergenos")]
        public async Task<ActionResult<List<string>>> GetIngredientesAlergenos()
        {
            try
            {
                var ingredientes = await _alergiaService.ObtenerIngredientesAlergenosAsync();
                return Ok(ingredientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingredientes alérgenos");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }

    // DTOs adicionales para operaciones específicas
    public class RegistrarAlergiaPorIdDto
    {
        public int NinoId { get; set; }
        public int IngredienteId { get; set; }
    }

    public class ActualizarAlergiaPorIdsDto
    {
        public int NinoId { get; set; }
        public List<int> NuevosIngredienteIds { get; set; } = new List<int>();
    }
}