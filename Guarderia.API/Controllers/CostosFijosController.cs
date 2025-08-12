using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Finanzas.CostoFijo;
using Guarderia.Domain.Entities;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CostosFijosController : ControllerBase
    {
        private readonly ICostoFijoMensualService _costoFijoService;
        private readonly ILogger<CostosFijosController> _logger;

        public CostosFijosController(
            ICostoFijoMensualService costoFijoService,
            ILogger<CostosFijosController> logger)
        {
            _costoFijoService = costoFijoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el costo fijo activo actualmente
        /// </summary>
        [HttpGet("activo")]
        public async Task<ActionResult<CostoFijoMensualDto>> GetActivo()
        {
            try
            {
                var costoFijo = await _costoFijoService.ObtenerActivoAsync();
                if (costoFijo == null)
                {
                    return NotFound("No hay ningún costo fijo activo configurado");
                }

                var costoFijoDto = MapToDto(costoFijo);
                return Ok(costoFijoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener costo fijo activo");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el monto vigente para una fecha específica
        /// </summary>
        [HttpGet("monto-vigente")]
        public async Task<ActionResult<decimal>> GetMontoVigente([FromQuery] DateTime? fecha = null)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Now;
                var monto = await _costoFijoService.ObtenerMontoVigenteAsync(fechaConsulta);
                return Ok(monto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener monto vigente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el historial completo de costos fijos
        /// </summary>
        [HttpGet("historial")]
        public async Task<ActionResult<List<CostoFijoMensualDto>>> GetHistorial()
        {
            try
            {
                var historial = await _costoFijoService.ObtenerHistorialAsync();
                var historialDto = historial.Select(c => MapToDto(c)).ToList();
                return Ok(historialDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de costos fijos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo costo fijo mensual
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CostoFijoMensualDto>> Create([FromBody] CrearCostoFijoDto crearCostoFijoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var costoFijoId = await _costoFijoService.CrearCostoFijoAsync(
                    crearCostoFijoDto.Monto,
                    crearCostoFijoDto.FechaVigencia,
                    crearCostoFijoDto.Descripcion);

                var costoFijoCreado = await _costoFijoService.ObtenerActivoAsync();
                var costoFijoDto = MapToDto(costoFijoCreado);

                return CreatedAtAction(nameof(GetById), new { id = costoFijoId }, costoFijoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear costo fijo");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un costo fijo por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CostoFijoMensualDto>> GetById(int id)
        {
            try
            {
                // Como no tenemos un método directo para obtener por ID en el servicio,
                // obtenemos el historial y buscamos por ID
                var historial = await _costoFijoService.ObtenerHistorialAsync();
                var costoFijo = historial.FirstOrDefault(c => c.Id == id);

                if (costoFijo == null)
                {
                    return NotFound($"No se encontró el costo fijo con ID {id}");
                }

                var costoFijoDto = MapToDto(costoFijo);
                return Ok(costoFijoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener costo fijo con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un costo fijo existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CostoFijoMensualDto>> Update(int id, [FromBody] CostoFijoMensualDto costoFijoDto)
        {
            try
            {
                if (id != costoFijoDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var costoFijo = new CostoFijoMensual
                {
                    Id = costoFijoDto.Id,
                    Monto = costoFijoDto.Monto,
                    FechaVigenciaDesde = costoFijoDto.FechaVigenciaDesde,
                    FechaVigenciaHasta = costoFijoDto.FechaVigenciaHasta,
                    Descripcion = costoFijoDto.Descripcion ?? string.Empty,
                    Activo = costoFijoDto.Activo
                };

                await _costoFijoService.ActualizarCostoFijoAsync(costoFijo);

                // Devolver el costo fijo actualizado
                var historial = await _costoFijoService.ObtenerHistorialAsync();
                var costoFijoActualizado = historial.FirstOrDefault(c => c.Id == id);
                var costoFijoActualizadoDto = MapToDto(costoFijoActualizado);

                return Ok(costoFijoActualizadoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar costo fijo con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Activa un nuevo costo fijo (desactiva el anterior automáticamente)
        /// </summary>
        [HttpPost("{id}/activar")]
        public async Task<ActionResult> ActivarNuevoCosto(int id, [FromBody] ActivarCostoFijoDto activarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _costoFijoService.ActivarNuevoCostoAsync(id, activarDto.FechaVigencia);
                return Ok(new { mensaje = "Costo fijo activado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar costo fijo con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Desactiva un costo fijo
        /// </summary>
        [HttpPost("{id}/desactivar")]
        public async Task<ActionResult> DesactivarCosto(int id)
        {
            try
            {
                await _costoFijoService.DesactivarCostoAsync(id);
                return Ok(new { mensaje = "Costo fijo desactivado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar costo fijo con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida un monto de costo fijo
        /// </summary>
        [HttpPost("validar-monto")]
        public async Task<ActionResult<bool>> ValidarMonto([FromBody] ValidarMontoDto validarDto)
        {
            try
            {
                var esValido = await _costoFijoService.ValidarMontoAsync(validarDto.Monto);
                return Ok(esValido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar monto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si existe un costo vigente para una fecha
        /// </summary>
        [HttpGet("tiene-costo-vigente")]
        public async Task<ActionResult<bool>> TieneCostoVigente([FromQuery] DateTime? fecha = null)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Now;
                var tieneCosto = await _costoFijoService.TieneCostoVigenteAsync(fechaConsulta);
                return Ok(tieneCosto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si tiene costo vigente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // Método auxiliar para mapear entidad a DTO
        private static CostoFijoMensualDto MapToDto(CostoFijoMensual costoFijo)
        {
            return new CostoFijoMensualDto
            {
                Id = costoFijo.Id,
                Monto = costoFijo.Monto,
                FechaVigenciaDesde = costoFijo.FechaVigenciaDesde,
                FechaVigenciaHasta = costoFijo.FechaVigenciaHasta,
                Descripcion = costoFijo.Descripcion,
                Activo = costoFijo.Activo
            };
        }
    }

    // DTOs adicionales para operaciones específicas
    public class ActivarCostoFijoDto
    {
        public DateTime FechaVigencia { get; set; }
    }

    public class ValidarMontoDto
    {
        public decimal Monto { get; set; }
    }
}