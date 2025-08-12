using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Main.Menu.Consumo;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumosMenuController : ControllerBase
    {
        private readonly IConsumoMenuService _consumoMenuService;
        private readonly ILogger<ConsumosMenuController> _logger;

        public ConsumosMenuController(
            IConsumoMenuService consumoMenuService,
            ILogger<ConsumosMenuController> logger)
        {
            _consumoMenuService = consumoMenuService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene los consumos de menú de un niño específico
        /// </summary>
        [HttpGet("nino/{ninoId}")]
        public async Task<ActionResult<List<ConsumoMenuDto>>> GetConsumosPorNino(int ninoId)
        {
            try
            {
                var consumos = await _consumoMenuService.ObtenerPorNiñoAsync(ninoId);
                var consumosDto = consumos.Select(c => new ConsumoMenuDto
                {
                    Id = c.Id,
                    NinoId = c.NinoId,
                    NinoNombre = c.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = c.Nino?.NumeroMatricula ?? string.Empty,
                    MenuId = c.MenuId,
                    MenuNombre = c.Menu?.Nombre ?? string.Empty,
                    Fecha = c.Fecha,
                    CostoReal = c.CostoReal,
                    Observaciones = c.Observaciones
                }).ToList();

                return Ok(consumosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener consumos de menú del niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los consumos de menú de una fecha específica
        /// </summary>
        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<List<ConsumoMenuDto>>> GetConsumosPorFecha(DateTime fecha)
        {
            try
            {
                var consumos = await _consumoMenuService.ObtenerPorFechaAsync(fecha);
                var consumosDto = consumos.Select(c => new ConsumoMenuDto
                {
                    Id = c.Id,
                    NinoId = c.NinoId,
                    NinoNombre = c.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = c.Nino?.NumeroMatricula ?? string.Empty,
                    MenuId = c.MenuId,
                    MenuNombre = c.Menu?.Nombre ?? string.Empty,
                    Fecha = c.Fecha,
                    CostoReal = c.CostoReal,
                    Observaciones = c.Observaciones
                }).ToList();

                return Ok(consumosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener consumos de menú de la fecha {Fecha}", fecha);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los consumos de menú del día actual
        /// </summary>
        [HttpGet("hoy")]
        public async Task<ActionResult<List<ConsumoMenuDto>>> GetConsumosHoy()
        {
            try
            {
                var hoy = DateTime.Now.Date;
                var consumos = await _consumoMenuService.ObtenerPorFechaAsync(hoy);
                var consumosDto = consumos.Select(c => new ConsumoMenuDto
                {
                    Id = c.Id,
                    NinoId = c.NinoId,
                    NinoNombre = c.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = c.Nino?.NumeroMatricula ?? string.Empty,
                    MenuId = c.MenuId,
                    MenuNombre = c.Menu?.Nombre ?? string.Empty,
                    Fecha = c.Fecha,
                    CostoReal = c.CostoReal,
                    Observaciones = c.Observaciones
                }).ToList();

                return Ok(consumosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener consumos de menú de hoy");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el consumo específico de un niño en una fecha
        /// </summary>
        [HttpGet("nino/{ninoId}/fecha/{fecha}")]
        public async Task<ActionResult<ConsumoMenuDto>> GetConsumoPorNinoYFecha(int ninoId, DateTime fecha)
        {
            try
            {
                var consumo = await _consumoMenuService.ObtenerPorNiñoYFechaAsync(ninoId, fecha);
                if (consumo == null)
                {
                    return NotFound($"No se encontró consumo de menú para el niño {ninoId} en la fecha {fecha:yyyy-MM-dd}");
                }

                var consumoDto = new ConsumoMenuDto
                {
                    Id = consumo.Id,
                    NinoId = consumo.NinoId,
                    NinoNombre = consumo.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = consumo.Nino?.NumeroMatricula ?? string.Empty,
                    MenuId = consumo.MenuId,
                    MenuNombre = consumo.Menu?.Nombre ?? string.Empty,
                    Fecha = consumo.Fecha,
                    CostoReal = consumo.CostoReal,
                    Observaciones = consumo.Observaciones
                };

                return Ok(consumoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener consumo de menú del niño {NinoId} en fecha {Fecha}", ninoId, fecha);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los consumos de menú de un niño en un mes específico
        /// </summary>
        [HttpGet("nino/{ninoId}/mes/{mes}/ano/{año}")]
        public async Task<ActionResult<List<ConsumoMenuDto>>> GetConsumosPorMes(int ninoId, int mes, int año)
        {
            try
            {
                if (mes < 1 || mes > 12)
                {
                    return BadRequest("El mes debe estar entre 1 y 12");
                }

                var consumos = await _consumoMenuService.ObtenerPorMesAsync(ninoId, mes, año);
                var consumosDto = consumos.Select(c => new ConsumoMenuDto
                {
                    Id = c.Id,
                    NinoId = c.NinoId,
                    NinoNombre = c.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = c.Nino?.NumeroMatricula ?? string.Empty,
                    MenuId = c.MenuId,
                    MenuNombre = c.Menu?.Nombre ?? string.Empty,
                    Fecha = c.Fecha,
                    CostoReal = c.CostoReal,
                    Observaciones = c.Observaciones
                }).ToList();

                return Ok(consumosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener consumos de menú del niño {NinoId} en {Mes}/{Año}", ninoId, mes, año);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Registra un nuevo consumo de menú
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> RegistrarConsumo([FromBody] RegistrarConsumoMenuDto registrarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el menú sea apropiado para el niño
                var esValido = await _consumoMenuService.ValidarConsumoParaNiñoAsync(registrarDto.NinoId, registrarDto.MenuId);
                if (!esValido)
                {
                    return BadRequest("El menú seleccionado contiene ingredientes a los que el niño es alérgico");
                }

                var consumoId = await _consumoMenuService.RegistrarConsumoAsync(
                    registrarDto.NinoId,
                    registrarDto.MenuId,
                    registrarDto.Fecha);

                return Ok(new { mensaje = "Consumo de menú registrado exitosamente", id = consumoId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar consumo de menú");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un consumo de menú existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarConsumo(int id, [FromBody] ConsumoMenuDto consumoDto)
        {
            try
            {
                if (id != consumoDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var consumo = new Domain.Entities.ConsumoMenu
                {
                    Id = consumoDto.Id,
                    NinoId = consumoDto.NinoId,
                    MenuId = consumoDto.MenuId,
                    Fecha = consumoDto.Fecha,
                    CostoReal = consumoDto.CostoReal,
                    Observaciones = consumoDto.Observaciones ?? string.Empty
                };

                await _consumoMenuService.ActualizarConsumoAsync(consumo);
                return Ok(new { mensaje = "Consumo de menú actualizado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar consumo de menú con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida si un menú es apropiado para un niño
        /// </summary>
        [HttpPost("validar")]
        public async Task<ActionResult<ValidacionMenuResultDto>> ValidarConsumoParaNino([FromBody] ValidarMenuParaNinoDto validarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var esValido = await _consumoMenuService.ValidarConsumoParaNiñoAsync(validarDto.NinoId, validarDto.MenuId);

                var resultado = new ValidacionMenuResultDto
                {
                    EsSeguro = esValido,
                    MensajeValidacion = esValido ? "El menú es seguro para el niño" : "El menú contiene ingredientes alérgenos para el niño"
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar consumo para niño");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Calcula el costo total de menús consumidos en un mes
        /// </summary>
        [HttpGet("nino/{ninoId}/costo-mensual/mes/{mes}/ano/{año}")]
        public async Task<ActionResult<decimal>> CalcularCostoMenusMensual(int ninoId, int mes, int año)
        {
            try
            {
                if (mes < 1 || mes > 12)
                {
                    return BadRequest("El mes debe estar entre 1 y 12");
                }

                var costo = await _consumoMenuService.CalcularCostoMenusMensualAsync(ninoId, mes, año);
                return Ok(costo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular costo de menús mensual del niño {NinoId} en {Mes}/{Año}", ninoId, mes, año);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Cuenta los días que un niño consumió comidas en un mes
        /// </summary>
        [HttpGet("nino/{ninoId}/dias-comidas/mes/{mes}/ano/{año}")]
        public async Task<ActionResult<int>> ContarDiasComidasMensual(int ninoId, int mes, int año)
        {
            try
            {
                if (mes < 1 || mes > 12)
                {
                    return BadRequest("El mes debe estar entre 1 y 12");
                }

                var dias = await _consumoMenuService.ContarDiasComidasMensualAsync(ninoId, mes, año);
                return Ok(dias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar días de comidas del niño {NinoId} en {Mes}/{Año}", ninoId, mes, año);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Genera reporte de consumo diario
        /// </summary>
        [HttpGet("reporte-diario")]
        public async Task<ActionResult<List<ConsumoMenuDto>>> GenerarReporteConsumoDiario([FromQuery] DateTime? fecha = null)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Now.Date;
                var consumos = await _consumoMenuService.GenerarReporteConsumoDiarioAsync(fechaConsulta);
                var consumosDto = consumos.Select(c => new ConsumoMenuDto
                {
                    Id = c.Id,
                    NinoId = c.NinoId,
                    NinoNombre = c.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = c.Nino?.NumeroMatricula ?? string.Empty,
                    MenuId = c.MenuId,
                    MenuNombre = c.Menu?.Nombre ?? string.Empty,
                    Fecha = c.Fecha,
                    CostoReal = c.CostoReal,
                    Observaciones = c.Observaciones
                }).ToList();

                return Ok(consumosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de consumo diario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Genera reporte de consumo mensual
        /// </summary>
        [HttpGet("reporte-mensual/mes/{mes}/ano/{año}")]
        public async Task<ActionResult<List<ConsumoMenuDto>>> GenerarReporteConsumoMensual(int mes, int año)
        {
            try
            {
                if (mes < 1 || mes > 12)
                {
                    return BadRequest("El mes debe estar entre 1 y 12");
                }

                var consumos = await _consumoMenuService.GenerarReporteConsumoMensualAsync(mes, año);
                var consumosDto = consumos.Select(c => new ConsumoMenuDto
                {
                    Id = c.Id,
                    NinoId = c.NinoId,
                    NinoNombre = c.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = c.Nino?.NumeroMatricula ?? string.Empty,
                    MenuId = c.MenuId,
                    MenuNombre = c.Menu?.Nombre ?? string.Empty,
                    Fecha = c.Fecha,
                    CostoReal = c.CostoReal,
                    Observaciones = c.Observaciones
                }).ToList();

                return Ok(consumosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de consumo mensual para {Mes}/{Año}", mes, año);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un consumo de menú
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarConsumo(int id)
        {
            try
            {
                await _consumoMenuService.EliminarConsumoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar consumo de menú con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}