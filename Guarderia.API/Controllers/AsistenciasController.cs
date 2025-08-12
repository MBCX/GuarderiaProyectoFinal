using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Main.Asistencia;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciasController : ControllerBase
    {
        private readonly IAsistenciaService _asistenciaService;
        private readonly ILogger<AsistenciasController> _logger;

        public AsistenciasController(IAsistenciaService asistenciaService, ILogger<AsistenciasController> logger)
        {
            _asistenciaService = asistenciaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene las asistencias de un niño específico
        /// </summary>
        [HttpGet("nino/{ninoId}")]
        public async Task<ActionResult<List<AsistenciaDto>>> GetAsistenciasPorNino(int ninoId)
        {
            try
            {
                var asistencias = await _asistenciaService.ObtenerAsistenciasPorNiñoAsync(ninoId);
                var asistenciasDto = asistencias.Select(a => new AsistenciaDto
                {
                    Id = a.Id,
                    NinoId = a.NinoId,
                    NinoNombre = a.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = a.Nino?.NumeroMatricula ?? string.Empty,
                    Fecha = a.Fecha,
                    Asistio = a.Asistio
                }).ToList();

                return Ok(asistenciasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias del niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las asistencias de una fecha específica
        /// </summary>
        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<List<AsistenciaDto>>> GetAsistenciasPorFecha(DateTime fecha)
        {
            try
            {
                var asistencias = await _asistenciaService.ObtenerAsistenciasPorFechaAsync(fecha);
                var asistenciasDto = asistencias.Select(a => new AsistenciaDto
                {
                    Id = a.Id,
                    NinoId = a.NinoId,
                    NinoNombre = a.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = a.Nino?.NumeroMatricula ?? string.Empty,
                    Fecha = a.Fecha,
                    Asistio = a.Asistio
                }).ToList();

                return Ok(asistenciasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias de la fecha {Fecha}", fecha);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las asistencias del día actual
        /// </summary>
        [HttpGet("hoy")]
        public async Task<ActionResult<List<AsistenciaDto>>> GetAsistenciasHoy()
        {
            try
            {
                var hoy = DateTime.Now.Date;
                var asistencias = await _asistenciaService.ObtenerAsistenciasPorFechaAsync(hoy);
                var asistenciasDto = asistencias.Select(a => new AsistenciaDto
                {
                    Id = a.Id,
                    NinoId = a.NinoId,
                    NinoNombre = a.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = a.Nino?.NumeroMatricula ?? string.Empty,
                    Fecha = a.Fecha,
                    Asistio = a.Asistio
                }).ToList();

                return Ok(asistenciasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias de hoy");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las asistencias de un mes específico
        /// </summary>
        [HttpGet("mes/{mes}/ano/{año}")]
        public async Task<ActionResult<List<AsistenciaDto>>> GetAsistenciasPorMes(int mes, int año)
        {
            try
            {
                if (mes < 1 || mes > 12)
                {
                    return BadRequest("El mes debe estar entre 1 y 12");
                }

                var asistencias = await _asistenciaService.ObtenerAsistenciasPorMesAsync(mes, año);
                var asistenciasDto = asistencias.Select(a => new AsistenciaDto
                {
                    Id = a.Id,
                    NinoId = a.NinoId,
                    NinoNombre = a.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = a.Nino?.NumeroMatricula ?? string.Empty,
                    Fecha = a.Fecha,
                    Asistio = a.Asistio
                }).ToList();

                return Ok(asistenciasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias del mes {Mes}/{Año}", mes, año);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una asistencia específica por niño y fecha
        /// </summary>
        [HttpGet("nino/{ninoId}/fecha/{fecha}")]
        public async Task<ActionResult<AsistenciaDto>> GetAsistenciaPorNinoYFecha(int ninoId, DateTime fecha)
        {
            try
            {
                var asistencia = await _asistenciaService.ObtenerAsistenciaPorNiñoYFechaAsync(ninoId, fecha);
                if (asistencia == null)
                {
                    return NotFound($"No se encontró asistencia para el niño {ninoId} en la fecha {fecha:yyyy-MM-dd}");
                }

                var asistenciaDto = new AsistenciaDto
                {
                    Id = asistencia.Id,
                    NinoId = asistencia.NinoId,
                    NinoNombre = asistencia.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = asistencia.Nino?.NumeroMatricula ?? string.Empty,
                    Fecha = asistencia.Fecha,
                    Asistio = asistencia.Asistio
                };

                return Ok(asistenciaDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencia del niño {NinoId} en fecha {Fecha}", ninoId, fecha);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Registra una nueva asistencia
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> RegistrarAsistencia([FromBody] RegistrarAsistenciaDto registrarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _asistenciaService.RegistrarAsistenciaAsync(registrarDto.NinoId, registrarDto.Fecha, registrarDto.Asistio);
                return Ok(new { mensaje = "Asistencia registrada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar asistencia");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una asistencia existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarAsistencia(int id, [FromBody] AsistenciaDto asistenciaDto)
        {
            try
            {
                if (id != asistenciaDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var asistencia = new Domain.Entities.Asistencia
                {
                    Id = asistenciaDto.Id,
                    NinoId = asistenciaDto.NinoId,
                    Fecha = asistenciaDto.Fecha,
                    Asistio = asistenciaDto.Asistio
                };

                await _asistenciaService.ActualizarAsistenciaAsync(asistencia);
                return Ok(new { mensaje = "Asistencia actualizada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar asistencia con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Calcula los días de asistencia de un niño en un mes
        /// </summary>
        [HttpGet("nino/{ninoId}/dias-asistencia/mes/{mes}/ano/{año}")]
        public async Task<ActionResult<int>> CalcularDiasAsistencia(int ninoId, int mes, int año)
        {
            try
            {
                if (mes < 1 || mes > 12)
                {
                    return BadRequest("El mes debe estar entre 1 y 12");
                }

                var diasAsistencia = await _asistenciaService.CalcularDiasAsistenciaDelMesAsync(ninoId, mes, año);
                return Ok(diasAsistencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular días de asistencia del niño {NinoId} en {Mes}/{Año}", ninoId, mes, año);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Calcula el porcentaje de asistencia de un niño en un mes
        /// </summary>
        [HttpGet("nino/{ninoId}/porcentaje-asistencia/mes/{mes}/ano/{año}")]
        public async Task<ActionResult<decimal>> CalcularPorcentajeAsistencia(int ninoId, int mes, int año)
        {
            try
            {
                if (mes < 1 || mes > 12)
                {
                    return BadRequest("El mes debe estar entre 1 y 12");
                }

                var porcentaje = await _asistenciaService.CalcularPorcentajeAsistenciaAsync(ninoId, mes, año);
                return Ok(porcentaje);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular porcentaje de asistencia del niño {NinoId} en {Mes}/{Año}", ninoId, mes, año);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Calcula el costo de asistencia mensual de un niño
        /// </summary>
        [HttpGet("nino/{ninoId}/costo-mensual/mes/{mes}/ano/{año}")]
        public async Task<ActionResult<decimal>> CalcularCostoAsistenciaMensual(int ninoId, int mes, int año)
        {
            try
            {
                if (mes < 1 || mes > 12)
                {
                    return BadRequest("El mes debe estar entre 1 y 12");
                }

                var costo = await _asistenciaService.CalcularCostoAsistenciaMensualAsync(ninoId, mes, año);
                return Ok(costo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular costo de asistencia mensual del niño {NinoId} en {Mes}/{Año}", ninoId, mes, año);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si ya existe una asistencia registrada
        /// </summary>
        [HttpGet("verificar-registro/nino/{ninoId}/fecha/{fecha}")]
        public async Task<ActionResult<bool>> VerificarRegistro(int ninoId, DateTime fecha)
        {
            try
            {
                var yaRegistrada = await _asistenciaService.YaRegistradaAsync(ninoId, fecha);
                return Ok(yaRegistrada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar registro de asistencia del niño {NinoId} en fecha {Fecha}", ninoId, fecha);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Genera reporte de asistencia por rango de fechas
        /// </summary>
        [HttpGet("reporte")]
        public async Task<ActionResult<List<AsistenciaDto>>> GenerarReporte([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaFin < fechaInicio)
                {
                    return BadRequest("La fecha fin no puede ser anterior a la fecha inicio");
                }

                var asistencias = await _asistenciaService.GenerarReporteAsistenciaAsync(fechaInicio, fechaFin);
                var asistenciasDto = asistencias.Select(a => new AsistenciaDto
                {
                    Id = a.Id,
                    NinoId = a.NinoId,
                    NinoNombre = a.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = a.Nino?.NumeroMatricula ?? string.Empty,
                    Fecha = a.Fecha,
                    Asistio = a.Asistio
                }).ToList();

                return Ok(asistenciasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de asistencias");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Genera reporte de asistencia mensual
        /// </summary>
        [HttpGet("reporte-mensual/mes/{mes}/ano/{año}")]
        public async Task<ActionResult<List<AsistenciaDto>>> GenerarReporteMensual(int mes, int año)
        {
            try
            {
                if (mes < 1 || mes > 12)
                {
                    return BadRequest("El mes debe estar entre 1 y 12");
                }

                var asistencias = await _asistenciaService.GenerarReporteAsistenciaMensualAsync(mes, año);
                var asistenciasDto = asistencias.Select(a => new AsistenciaDto
                {
                    Id = a.Id,
                    NinoId = a.NinoId,
                    NinoNombre = a.Nino?.Nombre ?? string.Empty,
                    NumeroMatricula = a.Nino?.NumeroMatricula ?? string.Empty,
                    Fecha = a.Fecha,
                    Asistio = a.Asistio
                }).ToList();

                return Ok(asistenciasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte mensual de asistencias para {Mes}/{Año}", mes, año);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Cuenta los asistentes del día
        /// </summary>
        [HttpGet("contar-asistentes")]
        public async Task<ActionResult<int>> ContarAsistentesDelDia([FromQuery] DateTime? fecha = null)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Now.Date;
                var cantidad = await _asistenciaService.ContarAsistentesDelDiaAsync(fechaConsulta);
                return Ok(cantidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar asistentes del día");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una asistencia
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarAsistencia(int id)
        {
            try
            {
                await _asistenciaService.EliminarAsistenciaAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar asistencia con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}