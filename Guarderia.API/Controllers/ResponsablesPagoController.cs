using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Main.Pago;
using Guarderia.Shared.DTO.Main.Nino;
using Guarderia.Domain.Entities;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsablesPagoController : ControllerBase
    {
        private readonly IResponsablePagoService _responsablePagoService;
        private readonly INinoService _ninoService;
        private readonly ILogger<ResponsablesPagoController> _logger;

        public ResponsablesPagoController(
            IResponsablePagoService responsablePagoService,
            INinoService ninoService,
            ILogger<ResponsablesPagoController> logger)
        {
            _responsablePagoService = responsablePagoService;
            _ninoService = ninoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los responsables de pago
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ResponsablePagoDto>>> GetTodos()
        {
            try
            {
                var responsables = await _responsablePagoService.ObtenerTodosAsync();
                var responsablesDto = new List<ResponsablePagoDto>();

                foreach (var responsable in responsables)
                {
                    var totalPendiente = await _responsablePagoService.ObtenerTotalPendienteAsync(responsable.Id);
                    var ninosAPagar = await _ninoService.ObtenerPorResponsablePagoAsync(responsable.Id);

                    responsablesDto.Add(new ResponsablePagoDto
                    {
                        Id = responsable.Id,
                        Cedula = responsable.Cedula,
                        Nombre = responsable.Nombre,
                        Direccion = responsable.Direccion,
                        Telefono = responsable.Telefono,
                        CuentaCorriente = responsable.CuentaCorriente,
                        TotalPendiente = totalPendiente,
                        NinosAPagar = ninosAPagar.Select(n => new NinoDto
                        {
                            Id = n.Id,
                            NumeroMatricula = n.NumeroMatricula,
                            Nombre = n.Nombre,
                            FechaNacimiento = n.FechaNacimiento,
                            FechaIngreso = n.FechaIngreso,
                            FechaBaja = n.FechaBaja,
                            Activo = n.Activo,
                            ResponsablePagoId = n.ResponsablePagoId
                        }).ToList()
                    });
                }

                return Ok(responsablesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los responsables de pago");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un responsable de pago por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponsablePagoDto>> GetById(int id)
        {
            try
            {
                var responsable = await _responsablePagoService.ObtenerPorIdAsync(id);
                if (responsable == null)
                {
                    return NotFound($"No se encontró el responsable de pago con ID {id}");
                }

                var totalPendiente = await _responsablePagoService.ObtenerTotalPendienteAsync(responsable.Id);
                var ninosAPagar = await _ninoService.ObtenerPorResponsablePagoAsync(responsable.Id);

                var responsableDto = new ResponsablePagoDto
                {
                    Id = responsable.Id,
                    Cedula = responsable.Cedula,
                    Nombre = responsable.Nombre,
                    Direccion = responsable.Direccion,
                    Telefono = responsable.Telefono,
                    CuentaCorriente = responsable.CuentaCorriente,
                    TotalPendiente = totalPendiente,
                    NinosAPagar = ninosAPagar.Select(n => new NinoDto
                    {
                        Id = n.Id,
                        NumeroMatricula = n.NumeroMatricula,
                        Nombre = n.Nombre,
                        FechaNacimiento = n.FechaNacimiento,
                        FechaIngreso = n.FechaIngreso,
                        FechaBaja = n.FechaBaja,
                        Activo = n.Activo,
                        ResponsablePagoId = n.ResponsablePagoId
                    }).ToList()
                };

                return Ok(responsableDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener responsable de pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un responsable de pago por su cédula
        /// </summary>
        [HttpGet("cedula/{cedula}")]
        public async Task<ActionResult<ResponsablePagoDto>> GetByCedula(string cedula)
        {
            try
            {
                var responsable = await _responsablePagoService.ObtenerPorCedulaAsync(cedula);
                if (responsable == null)
                {
                    return NotFound($"No se encontró responsable de pago con cédula {cedula}");
                }

                var totalPendiente = await _responsablePagoService.ObtenerTotalPendienteAsync(responsable.Id);
                var ninosAPagar = await _ninoService.ObtenerPorResponsablePagoAsync(responsable.Id);

                var responsableDto = new ResponsablePagoDto
                {
                    Id = responsable.Id,
                    Cedula = responsable.Cedula,
                    Nombre = responsable.Nombre,
                    Direccion = responsable.Direccion,
                    Telefono = responsable.Telefono,
                    CuentaCorriente = responsable.CuentaCorriente,
                    TotalPendiente = totalPendiente,
                    NinosAPagar = ninosAPagar.Select(n => new NinoDto
                    {
                        Id = n.Id,
                        NumeroMatricula = n.NumeroMatricula,
                        Nombre = n.Nombre,
                        FechaNacimiento = n.FechaNacimiento,
                        FechaIngreso = n.FechaIngreso,
                        FechaBaja = n.FechaBaja,
                        Activo = n.Activo,
                        ResponsablePagoId = n.ResponsablePagoId
                    }).ToList()
                };

                return Ok(responsableDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener responsable de pago con cédula {Cedula}", cedula);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el responsable de pago de un niño específico
        /// </summary>
        [HttpGet("nino/{ninoId}")]
        public async Task<ActionResult<ResponsablePagoDto>> GetPorNino(int ninoId)
        {
            try
            {
                var responsable = await _responsablePagoService.ObtenerPorNiñoAsync(ninoId);
                if (responsable == null)
                {
                    return NotFound($"No se encontró responsable de pago para el niño con ID {ninoId}");
                }

                var totalPendiente = await _responsablePagoService.ObtenerTotalPendienteAsync(responsable.Id);
                var ninosAPagar = await _ninoService.ObtenerPorResponsablePagoAsync(responsable.Id);

                var responsableDto = new ResponsablePagoDto
                {
                    Id = responsable.Id,
                    Cedula = responsable.Cedula,
                    Nombre = responsable.Nombre,
                    Direccion = responsable.Direccion,
                    Telefono = responsable.Telefono,
                    CuentaCorriente = responsable.CuentaCorriente,
                    TotalPendiente = totalPendiente,
                    NinosAPagar = ninosAPagar.Select(n => new NinoDto
                    {
                        Id = n.Id,
                        NumeroMatricula = n.NumeroMatricula,
                        Nombre = n.Nombre,
                        FechaNacimiento = n.FechaNacimiento,
                        FechaIngreso = n.FechaIngreso,
                        FechaBaja = n.FechaBaja,
                        Activo = n.Activo,
                        ResponsablePagoId = n.ResponsablePagoId
                    }).ToList()
                };

                return Ok(responsableDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener responsable de pago del niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Registra un nuevo responsable de pago
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResponsablePagoDto>> Create([FromBody] CrearResponsablePagoDto crearResponsableDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var responsable = new ResponsablePago
                {
                    Cedula = crearResponsableDto.Cedula,
                    Nombre = crearResponsableDto.Nombre,
                    Direccion = crearResponsableDto.Direccion ?? string.Empty,
                    Telefono = crearResponsableDto.Telefono ?? string.Empty,
                    CuentaCorriente = crearResponsableDto.CuentaCorriente ?? string.Empty
                };

                var responsableId = await _responsablePagoService.RegistrarResponsablePagoAsync(responsable);
                var responsableCreado = await _responsablePagoService.ObtenerPorIdAsync(responsableId);

                var responsableDto = new ResponsablePagoDto
                {
                    Id = responsableCreado.Id,
                    Cedula = responsableCreado.Cedula,
                    Nombre = responsableCreado.Nombre,
                    Direccion = responsableCreado.Direccion,
                    Telefono = responsableCreado.Telefono,
                    CuentaCorriente = responsableCreado.CuentaCorriente,
                    TotalPendiente = 0,
                    NinosAPagar = new List<NinoDto>()
                };

                return CreatedAtAction(nameof(GetById), new { id = responsableId }, responsableDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear responsable de pago");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza los datos de un responsable de pago
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponsablePagoDto>> Update(int id, [FromBody] ResponsablePagoDto responsableDto)
        {
            try
            {
                if (id != responsableDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var responsable = new ResponsablePago
                {
                    Id = responsableDto.Id,
                    Cedula = responsableDto.Cedula,
                    Nombre = responsableDto.Nombre,
                    Direccion = responsableDto.Direccion ?? string.Empty,
                    Telefono = responsableDto.Telefono ?? string.Empty,
                    CuentaCorriente = responsableDto.CuentaCorriente ?? string.Empty
                };

                await _responsablePagoService.ActualizarResponsablePagoAsync(responsable);

                var responsableActualizado = await _responsablePagoService.ObtenerPorIdAsync(id);
                var totalPendiente = await _responsablePagoService.ObtenerTotalPendienteAsync(id);
                var ninosAPagar = await _ninoService.ObtenerPorResponsablePagoAsync(id);

                var responsableActualizadoDto = new ResponsablePagoDto
                {
                    Id = responsableActualizado.Id,
                    Cedula = responsableActualizado.Cedula,
                    Nombre = responsableActualizado.Nombre,
                    Direccion = responsableActualizado.Direccion,
                    Telefono = responsableActualizado.Telefono,
                    CuentaCorriente = responsableActualizado.CuentaCorriente,
                    TotalPendiente = totalPendiente,
                    NinosAPagar = ninosAPagar.Select(n => new NinoDto
                    {
                        Id = n.Id,
                        NumeroMatricula = n.NumeroMatricula,
                        Nombre = n.Nombre,
                        FechaNacimiento = n.FechaNacimiento,
                        FechaIngreso = n.FechaIngreso,
                        FechaBaja = n.FechaBaja,
                        Activo = n.Activo,
                        ResponsablePagoId = n.ResponsablePagoId
                    }).ToList()
                };

                return Ok(responsableActualizadoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar responsable de pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Asigna un responsable de pago a un niño
        /// </summary>
        [HttpPost("{responsableId}/asignar-nino/{ninoId}")]
        public async Task<ActionResult> AsignarANino(int responsableId, int ninoId)
        {
            try
            {
                var responsable = await _responsablePagoService.ObtenerPorIdAsync(responsableId);
                if (responsable == null)
                {
                    return NotFound($"No se encontró el responsable de pago con ID {responsableId}");
                }

                var nino = await _ninoService.ObtenerNinoPorIdAsync(ninoId);
                if (nino == null)
                {
                    return NotFound($"No se encontró el niño con ID {ninoId}");
                }

                await _responsablePagoService.AsignarANiñoAsync(responsableId, ninoId);
                return Ok(new { mensaje = "Responsable asignado exitosamente al niño" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar responsable {ResponsableId} al niño {NinoId}", responsableId, ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida los datos obligatorios de un responsable de pago
        /// </summary>
        [HttpPost("validar-datos")]
        public async Task<ActionResult<bool>> ValidarDatos([FromBody] ResponsablePago responsable)
        {
            try
            {
                var sonValidos = await _responsablePagoService.ValidarDatosObligatoriosAsync(responsable);
                return Ok(sonValidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar datos del responsable de pago");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida una cuenta corriente
        /// </summary>
        [HttpGet("validar-cuenta/{cuentaCorriente}")]
        public async Task<ActionResult<bool>> ValidarCuentaCorriente(string cuentaCorriente)
        {
            try
            {
                var esValida = await _responsablePagoService.ValidarCuentaCorrienteAsync(cuentaCorriente);
                return Ok(esValida);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar cuenta corriente {CuentaCorriente}", cuentaCorriente);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el total pendiente de pago de un responsable
        /// </summary>
        [HttpGet("{id}/total-pendiente")]
        public async Task<ActionResult<decimal>> GetTotalPendiente(int id)
        {
            try
            {
                var responsable = await _responsablePagoService.ObtenerPorIdAsync(id);
                if (responsable == null)
                {
                    return NotFound($"No se encontró el responsable de pago con ID {id}");
                }

                var totalPendiente = await _responsablePagoService.ObtenerTotalPendienteAsync(id);
                return Ok(totalPendiente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total pendiente del responsable {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los niños asignados a un responsable de pago
        /// </summary>
        [HttpGet("{id}/ninos")]
        public async Task<ActionResult<List<NinoDto>>> GetNinosAsignados(int id)
        {
            try
            {
                var responsable = await _responsablePagoService.ObtenerPorIdAsync(id);
                if (responsable == null)
                {
                    return NotFound($"No se encontró el responsable de pago con ID {id}");
                }

                var ninos = await _ninoService.ObtenerPorResponsablePagoAsync(id);
                var ninosDto = ninos.Select(n => new NinoDto
                {
                    Id = n.Id,
                    NumeroMatricula = n.NumeroMatricula,
                    Nombre = n.Nombre,
                    FechaNacimiento = n.FechaNacimiento,
                    FechaIngreso = n.FechaIngreso,
                    FechaBaja = n.FechaBaja,
                    Activo = n.Activo,
                    ResponsablePagoId = n.ResponsablePagoId
                }).ToList();

                return Ok(ninosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener niños del responsable {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un responsable de pago del sistema
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var responsable = await _responsablePagoService.ObtenerPorIdAsync(id);
                if (responsable == null)
                {
                    return NotFound($"No se encontró el responsable de pago con ID {id}");
                }

                // Verificar que no tenga niños asignados antes de eliminar
                var ninosAsignados = await _ninoService.ObtenerPorResponsablePagoAsync(id);
                if (ninosAsignados.Any())
                {
                    return BadRequest("No se puede eliminar el responsable porque tiene niños asignados. Primero debe reasignar o dar de baja a los niños.");
                }

                await _responsablePagoService.EliminarResponsablePagoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar responsable de pago con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Busca responsables de pago por término de búsqueda
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<List<ResponsablePagoDto>>> Buscar([FromQuery] string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                {
                    return BadRequest("El término de búsqueda es obligatorio");
                }

                var todosLosResponsables = await _responsablePagoService.ObtenerTodosAsync();
                var responsablesFiltrados = todosLosResponsables.Where(r =>
                    r.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                    r.Cedula.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                    (r.Telefono?.Contains(termino, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();

                var responsablesDto = new List<ResponsablePagoDto>();
                foreach (var responsable in responsablesFiltrados)
                {
                    var totalPendiente = await _responsablePagoService.ObtenerTotalPendienteAsync(responsable.Id);
                    var ninosAPagar = await _ninoService.ObtenerPorResponsablePagoAsync(responsable.Id);

                    responsablesDto.Add(new ResponsablePagoDto
                    {
                        Id = responsable.Id,
                        Cedula = responsable.Cedula,
                        Nombre = responsable.Nombre,
                        Direccion = responsable.Direccion,
                        Telefono = responsable.Telefono,
                        CuentaCorriente = responsable.CuentaCorriente,
                        TotalPendiente = totalPendiente,
                        NinosAPagar = ninosAPagar.Select(n => new NinoDto
                        {
                            Id = n.Id,
                            NumeroMatricula = n.NumeroMatricula,
                            Nombre = n.Nombre,
                            FechaNacimiento = n.FechaNacimiento,
                            FechaIngreso = n.FechaIngreso,
                            FechaBaja = n.FechaBaja,
                            Activo = n.Activo,
                            ResponsablePagoId = n.ResponsablePagoId
                        }).ToList()
                    });
                }

                return Ok(responsablesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar responsables de pago con término {Termino}", termino);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}