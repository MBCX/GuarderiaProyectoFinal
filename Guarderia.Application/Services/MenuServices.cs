using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Application.Interfaces;
using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;

namespace Guarderia.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IPlatoRepository _platoRepository;
        private readonly IMenuPlatoRepository _menuPlatoRepository;
        private readonly IAlergiaRepository _alergiaRepository;

        public MenuService(
            IMenuRepository menuRepository,
            IPlatoRepository platoRepository,
            IMenuPlatoRepository menuPlatoRepository,
            IAlergiaRepository alergiaRepository = null)
        {
            _menuRepository = menuRepository;
            _platoRepository = platoRepository;
            _menuPlatoRepository = menuPlatoRepository;
            _alergiaRepository = alergiaRepository;
        }

        public async Task<Menu> ObtenerPorIdAsync(int id)
        {
            return await _menuRepository.ObtenerPorIdAsync(id);
        }

        public async Task<List<Menu>> ObtenerTodosAsync()
        {
            return await _menuRepository.ObtenerTodosAsync();
        }

        public async Task<List<Menu>> ObtenerActivosAsync()
        {
            return await _menuRepository.ObtenerActivosAsync();
        }

        public async Task<Menu> ObtenerMenuDelDiaAsync(DateTime fecha)
        {
            return await _menuRepository.ObtenerMenuDelDiaAsync(fecha);
        }

        public async Task<List<Menu>> ObtenerMenusSinAlergenosParaNiñoAsync(int ninoId)
        {
            return await _menuRepository.ObtenerMenusSinAlergenosParaNinoAsync(ninoId);
        }

        public async Task<int> CrearMenuAsync(Menu menu, List<int> platoIds)
        {
            // Validaciones
            if (menu == null)
            {
                throw new ArgumentException("El menú no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(menu.Nombre))
            {
                throw new ArgumentException("El nombre del menú es obligatorio");
            }

            if (menu.Precio <= 0)
            {
                throw new ArgumentException("El precio del menú debe ser mayor a 0");
            }

            if (platoIds == null || !platoIds.Any())
            {
                throw new ArgumentException("El menú debe tener al menos un plato");
            }

            // Verificar que todos los platos existen
            var platos = new List<Plato>();
            foreach (var platoId in platoIds)
            {
                var plato = await _platoRepository.ObtenerPorIdAsync(platoId);
                if (plato == null)
                {
                    throw new ArgumentException($"El plato con ID {platoId} no existe");
                }
                platos.Add(plato);
            }

            // Configurar valores por defecto
            menu.FechaCreacion = DateTime.Now;
            menu.Activo = true;
            menu.Nombre = menu.Nombre.Trim();

            await _menuRepository.AgregarAsync(menu);

            // Agregar platos al menú
            int orden = 1;
            foreach (var platoId in platoIds)
            {
                await AgregarPlatoAMenuAsync(menu.Id, platoId, orden);
                orden++;
            }

            return menu.Id;
        }

        public async Task ActualizarMenuAsync(Menu menu)
        {
            if (menu == null)
            {
                throw new ArgumentException("El menú no puede ser nulo");
            }

            var menuExistente = await _menuRepository.ObtenerPorIdAsync(menu.Id);
            if (menuExistente == null)
            {
                throw new ArgumentException("El menú especificado no existe");
            }

            // Validaciones
            if (string.IsNullOrWhiteSpace(menu.Nombre))
            {
                throw new ArgumentException("El nombre del menú es obligatorio");
            }

            if (menu.Precio <= 0)
            {
                throw new ArgumentException("El precio del menú debe ser mayor a 0");
            }

            menu.Nombre = menu.Nombre.Trim();
            await _menuRepository.ActualizarAsync(menu);
        }

        public async Task AgregarPlatoAMenuAsync(int menuId, int platoId, int orden)
        {
            var menu = await _menuRepository.ObtenerPorIdAsync(menuId);
            if (menu == null)
            {
                throw new ArgumentException("El menú especificado no existe");
            }

            var plato = await _platoRepository.ObtenerPorIdAsync(platoId);
            if (plato == null)
            {
                throw new ArgumentException("El plato especificado no existe");
            }

            var relacionExistente = await _menuPlatoRepository.ObtenerRelacionAsync(menuId, platoId);
            if (relacionExistente != null)
            {
                throw new InvalidOperationException("El plato ya está incluido en el menú");
            }

            var menuPlato = new MenuPlato
            {
                MenuId = menuId,
                PlatoId = platoId,
                Orden = orden,
                // El primer plato es considerado principal
                EsPlatoPrincipal = orden == 1
            };

            await _menuPlatoRepository.AgregarAsync(menuPlato);
        }

        public async Task RemoverPlatoDeMenuAsync(int menuId, int platoId)
        {
            var relacion = await _menuPlatoRepository.ObtenerRelacionAsync(menuId, platoId);
            if (relacion == null)
            {
                throw new ArgumentException("El plato no está incluido en el menú");
            }

            // Verificar que no sea el único plato del menú
            var platosEnMenu = await _menuPlatoRepository.ContarPlatosEnMenuAsync(menuId);
            if (platosEnMenu <= 1)
            {
                throw new InvalidOperationException("No se puede eliminar el último plato del menú");
            }

            await _menuPlatoRepository.EliminarAsync(menuId, platoId);
        }

        public async Task<bool> MenuTieneIngredienteAlergenoAsync(int menuId, int ninoId)
        {
            if (_alergiaRepository == null)
            {
                // Sin acceso a alergias, asumimos que es seguro
                return false;
            }

            var menu = await _menuRepository.ObtenerPorIdAsync(menuId);
            if (menu == null)
            {
                return false;
            }

            // Obtener alergias del niño
            var alergias = await _alergiaRepository.GetIngredientesAlergenosPorNinoAsync(ninoId);
            if (!alergias.Any())
            {
                return false;
            }

            return await _menuRepository.TieneIngredienteAlergenoAsync(menuId, alergias);
        }

        public async Task<List<string>> ObtenerIngredientesDeMenuAsync(int menuId)
        {
            var menu = await _menuRepository.ObtenerPorIdAsync(menuId);
            if (menu == null)
            {
                return new List<string>();
            }

            var ingredientes = new List<string>();

            foreach (var menuPlato in menu.Platos)
            {
                var plato = await _platoRepository.ObtenerPorIdAsync(menuPlato.PlatoId);
                if (plato != null)
                {
                    var ingredientesDelPlato = await _platoRepository.ObtenerIngredientesDePlatoAsync(plato.Id);
                    ingredientes.AddRange(ingredientesDelPlato);
                }
            }

            return ingredientes.Distinct().ToList();
        }

        public async Task<bool> ValidarMenuParaNiñoAsync(int menuId, int ninoId)
        {
            return !await MenuTieneIngredienteAlergenoAsync(menuId, ninoId);
        }

        public async Task ActivarMenuAsync(int id)
        {
            var menu = await _menuRepository.ObtenerPorIdAsync(id);
            if (menu == null)
            {
                throw new ArgumentException("El menú especificado no existe");
            }

            if (!menu.Activo)
            {
                menu.Activo = true;
                await _menuRepository.ActualizarAsync(menu);
            }
        }

        public async Task DesactivarMenuAsync(int id)
        {
            var menu = await _menuRepository.ObtenerPorIdAsync(id);
            if (menu == null)
            {
                throw new ArgumentException("El menú especificado no existe");
            }

            if (menu.Activo)
            {
                menu.Activo = false;
                await _menuRepository.ActualizarAsync(menu);
            }
        }
    }
}