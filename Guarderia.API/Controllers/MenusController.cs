using Microsoft.AspNetCore.Mvc;
using Guarderia.Application.Interfaces;
using Guarderia.Shared.DTO.Main.Menu;
using Guarderia.Shared.DTO.Main.Menu.Plato;
using Guarderia.Shared.DTO.Main.Menu.Ingrediente;
using Guarderia.Domain.Entities;

namespace Guarderia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IPlatoService _platoService;
        private readonly ILogger<MenusController> _logger;

        public MenusController(
            IMenuService menuService,
            IPlatoService platoService,
            ILogger<MenusController> logger)
        {
            _menuService = menuService;
            _platoService = platoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los menús
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<MenuDto>>> GetTodos()
        {
            try
            {
                var menus = await _menuService.ObtenerTodosAsync();
                var menusDto = await Task.WhenAll(menus.Select(async m => await MapToDtoAsync(m)));
                return Ok(menusDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los menús");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los menús activos
        /// </summary>
        [HttpGet("activos")]
        public async Task<ActionResult<List<MenuDto>>> GetActivos()
        {
            try
            {
                var menus = await _menuService.ObtenerActivosAsync();
                var menusDto = await Task.WhenAll(menus.Select(async m => await MapToDtoAsync(m)));
                return Ok(menusDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menús activos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un menú por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuDto>> GetById(int id)
        {
            try
            {
                var menu = await _menuService.ObtenerPorIdAsync(id);
                if (menu == null)
                {
                    return NotFound($"No se encontró el menú con ID {id}");
                }

                var menuDto = await MapToDtoAsync(menu);
                return Ok(menuDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menú con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el menú del día específico
        /// </summary>
        [HttpGet("del-dia")]
        public async Task<ActionResult<MenuDto>> GetMenuDelDia([FromQuery] DateTime? fecha = null)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Now;
                var menu = await _menuService.ObtenerMenuDelDiaAsync(fechaConsulta);

                if (menu == null)
                {
                    return NotFound($"No se encontró menú para la fecha {fechaConsulta:yyyy-MM-dd}");
                }

                var menuDto = await MapToDtoAsync(menu);
                return Ok(menuDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menú del día");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los menús sin alérgenos para un niño específico
        /// </summary>
        [HttpGet("sin-alergenos/nino/{ninoId}")]
        public async Task<ActionResult<List<MenuDto>>> GetMenusSinAlergenosParaNino(int ninoId)
        {
            try
            {
                var menus = await _menuService.ObtenerMenusSinAlergenosParaNiñoAsync(ninoId);
                var menusDto = await Task.WhenAll(menus.Select(async m => await MapToDtoAsync(m)));
                return Ok(menusDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menús sin alérgenos para el niño {NinoId}", ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo menú
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MenuDto>> Create([FromBody] CrearMenuDto crearMenuDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var menu = new Menu
                {
                    Nombre = crearMenuDto.Nombre,
                    Descripcion = crearMenuDto.Descripcion ?? string.Empty,
                    Precio = crearMenuDto.Precio,
                    FechaCreacion = DateTime.Now,
                    Activo = true
                };

                var menuId = await _menuService.CrearMenuAsync(menu, crearMenuDto.PlatoIds);
                var menuCreado = await _menuService.ObtenerPorIdAsync(menuId);

                var menuDto = await MapToDtoAsync(menuCreado);
                return CreatedAtAction(nameof(GetById), new { id = menuId }, menuDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear menú");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un menú existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<MenuDto>> Update(int id, [FromBody] MenuDto menuDto)
        {
            try
            {
                if (id != menuDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var menu = new Menu
                {
                    Id = menuDto.Id,
                    Nombre = menuDto.Nombre,
                    Descripcion = menuDto.Descripcion ?? string.Empty,
                    Precio = menuDto.Precio,
                    FechaCreacion = menuDto.FechaCreacion,
                    Activo = menuDto.Activo
                };

                await _menuService.ActualizarMenuAsync(menu);

                var menuActualizado = await _menuService.ObtenerPorIdAsync(id);
                var menuActualizadoDto = await MapToDtoAsync(menuActualizado);
                return Ok(menuActualizadoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar menú con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Agrega un plato a un menú
        /// </summary>
        [HttpPost("{menuId}/platos/{platoId}")]
        public async Task<ActionResult> AgregarPlato(int menuId, int platoId, [FromBody] AgregarPlatoDto agregarDto)
        {
            try
            {
                var menu = await _menuService.ObtenerPorIdAsync(menuId);
                if (menu == null)
                {
                    return NotFound($"No se encontró el menú con ID {menuId}");
                }

                var plato = await _platoService.ObtenerPorIdAsync(platoId);
                if (plato == null)
                {
                    return NotFound($"No se encontró el plato con ID {platoId}");
                }

                await _menuService.AgregarPlatoAMenuAsync(menuId, platoId, agregarDto.Orden);
                return Ok(new { mensaje = "Plato agregado exitosamente al menú" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar plato {PlatoId} al menú {MenuId}", platoId, menuId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Remueve un plato de un menú
        /// </summary>
        [HttpDelete("{menuId}/platos/{platoId}")]
        public async Task<ActionResult> RemoverPlato(int menuId, int platoId)
        {
            try
            {
                var menu = await _menuService.ObtenerPorIdAsync(menuId);
                if (menu == null)
                {
                    return NotFound($"No se encontró el menú con ID {menuId}");
                }

                await _menuService.RemoverPlatoDeMenuAsync(menuId, platoId);
                return Ok(new { mensaje = "Plato removido exitosamente del menú" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover plato {PlatoId} del menú {MenuId}", platoId, menuId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un menú tiene ingredientes alérgenos para un niño
        /// </summary>
        [HttpGet("{id}/tiene-alergenos/nino/{ninoId}")]
        public async Task<ActionResult<bool>> TieneIngredienteAlergeno(int id, int ninoId)
        {
            try
            {
                var tieneAlergeno = await _menuService.MenuTieneIngredienteAlergenoAsync(id, ninoId);
                return Ok(tieneAlergeno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar alérgenos del menú {MenuId} para el niño {NinoId}", id, ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los ingredientes de un menú
        /// </summary>
        [HttpGet("{id}/ingredientes")]
        public async Task<ActionResult<List<string>>> GetIngredientes(int id)
        {
            try
            {
                var menu = await _menuService.ObtenerPorIdAsync(id);
                if (menu == null)
                {
                    return NotFound($"No se encontró el menú con ID {id}");
                }

                var ingredientes = await _menuService.ObtenerIngredientesDeMenuAsync(id);
                return Ok(ingredientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingredientes del menú {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida si un menú es apropiado para un niño
        /// </summary>
        [HttpGet("{id}/validar-para-nino/{ninoId}")]
        public async Task<ActionResult<bool>> ValidarParaNino(int id, int ninoId)
        {
            try
            {
                var esValido = await _menuService.ValidarMenuParaNiñoAsync(id, ninoId);
                return Ok(esValido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar menú {MenuId} para el niño {NinoId}", id, ninoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Activa un menú
        /// </summary>
        [HttpPatch("{id}/activar")]
        public async Task<ActionResult> Activar(int id)
        {
            try
            {
                var menu = await _menuService.ObtenerPorIdAsync(id);
                if (menu == null)
                {
                    return NotFound($"No se encontró el menú con ID {id}");
                }

                await _menuService.ActivarMenuAsync(id);
                return Ok(new { mensaje = "Menú activado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar menú con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Desactiva un menú
        /// </summary>
        [HttpPatch("{id}/desactivar")]
        public async Task<ActionResult> Desactivar(int id)
        {
            try
            {
                var menu = await _menuService.ObtenerPorIdAsync(id);
                if (menu == null)
                {
                    return NotFound($"No se encontró el menú con ID {id}");
                }

                await _menuService.DesactivarMenuAsync(id);
                return Ok(new { mensaje = "Menú desactivado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar menú con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // Método auxiliar para mapear entidad a DTO
        private async Task<MenuDto> MapToDtoAsync(Menu menu)
        {
            var platos = await _platoService.ObtenerPorMenuAsync(menu.Id);
            var ingredientes = await _menuService.ObtenerIngredientesDeMenuAsync(menu.Id);

            return new MenuDto
            {
                Id = menu.Id,
                Nombre = menu.Nombre,
                Descripcion = menu.Descripcion,
                Precio = menu.Precio,
                FechaCreacion = menu.FechaCreacion,
                Activo = menu.Activo,
                Platos = platos.Select(p => new PlatoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    TipoPlato = p.TipoPlato
                }).ToList(),
                IngredientesDelMenu = ingredientes
            };
        }
    }

    // DTO auxiliar para agregar platos
    public class AgregarPlatoDto
    {
        public int Orden { get; set; } = 0;
    }
}