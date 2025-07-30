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
    public class ConsumoMenuService : IConsumoMenuService
    {
        private readonly IConsumoMenuRepository _consumoMenuRepository;
        private readonly INinoRepository _ninoRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IAlergiaService _alergiaService;

        public ConsumoMenuService(
            IConsumoMenuRepository consumoMenuRepository,
            INinoRepository ninoRepository,
            IMenuRepository menuRepository,
            IAlergiaService alergiaService = null)
        {
            _consumoMenuRepository = consumoMenuRepository;
            _ninoRepository = ninoRepository;
            _menuRepository = menuRepository;
            _alergiaService = alergiaService;
        }

        public async Task<List<ConsumoMenu>> ObtenerPorNiñoAsync(int ninoId)
        {
            return await _consumoMenuRepository.ObtenerPorNinoIdAsync(ninoId);
        }

        public async Task<List<ConsumoMenu>> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _consumoMenuRepository.ObtenerPorFechaAsync(fecha);
        }

        public async Task<ConsumoMenu> ObtenerPorNiñoYFechaAsync(int ninoId, DateTime fecha)
        {
            return await _consumoMenuRepository.ObtenerPorNinoYFechaAsync(ninoId, fecha);
        }

        public async Task<List<ConsumoMenu>> ObtenerPorMesAsync(int ninoId, int mes, int año)
        {
            return await _consumoMenuRepository.ObtenerPorNinoYMesAsync(ninoId, mes, año);
        }

        public async Task<int> RegistrarConsumoAsync(int ninoId, int menuId, DateTime fecha)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("El niño especificado no existe");
            }

            if (!nino.Activo)
            {
                throw new InvalidOperationException("No se puede registrar consumo para un niño inactivo");
            }

            var menu = await _menuRepository.ObtenerPorIdAsync(menuId);
            if (menu == null)
            {
                throw new ArgumentException("El menú especificado no existe");
            }

            if (!menu.Activo)
            {
                throw new InvalidOperationException("No se puede asignar un menú inactivo");
            }

            if (fecha.Date > DateTime.Now.Date)
            {
                throw new ArgumentException("No se puede registrar consumo para fechas futuras");
            }

            if (fecha.Date < nino.FechaIngreso.Date)
            {
                throw new ArgumentException("No se puede registrar consumo anterior a la fecha de ingreso");
            }

            var consumoExistente = await ObtenerPorNiñoYFechaAsync(ninoId, fecha);
            if (consumoExistente != null)
            {
                throw new InvalidOperationException($"Ya existe un registro de consumo de menú para el niño en la fecha {fecha:dd/MM/yyyy}");
            }

            if (!await ValidarConsumoParaNiñoAsync(ninoId, menuId))
            {
                throw new InvalidOperationException("El menú contiene ingredientes a los que el niño es alérgico");
            }

            var consumo = new ConsumoMenu
            {
                NinoId = ninoId,
                MenuId = menuId,
                Fecha = fecha.Date,
                CostoReal = menu.Precio,
                Observaciones = "",
                Nino = nino,
                Menu = menu
            };

            await _consumoMenuRepository.RegistrarAsync(consumo);
            return consumo.Id;
        }

        public async Task ActualizarConsumoAsync(ConsumoMenu consumo)
        {
            if (consumo == null)
            {
                throw new ArgumentException("El consumo no puede ser nulo");
            }

            var consumoExistente = await _consumoMenuRepository.ObtenerPorIdAsync(consumo.Id);
            if (consumoExistente == null)
            {
                throw new ArgumentException("El consumo especificado no existe");
            }

            // Si se cambia el menu, validar alergias nuevamente
            if (consumoExistente.MenuId != consumo.MenuId)
            {
                if (!await ValidarConsumoParaNiñoAsync(consumo.NinoId, consumo.MenuId))
                {
                    throw new InvalidOperationException("El nuevo menú contiene ingredientes a los que el niño es alérgico");
                }

                // Actualizar el costo real con el precio del nuevo menu
                var menu = await _menuRepository.ObtenerPorIdAsync(consumo.MenuId);
                if (menu != null)
                {
                    consumo.CostoReal = menu.Precio;
                }
            }

            await _consumoMenuRepository.ActualizarAsync(consumo);
        }

        public async Task<bool> ValidarConsumoParaNiñoAsync(int ninoId, int menuId)
        {
            if (_alergiaService == null)
            {
                // Si no tenemos servicio de alergias, asumimos que es seguro
                return true;
            }

            return await _alergiaService.PuedeConsumirMenuAsync(ninoId, menuId);
        }

        public async Task<decimal> CalcularCostoMenusMensualAsync(int ninoId, int mes, int año)
        {
            return await _consumoMenuRepository.CalcularCostoMenusMensualAsync(ninoId, mes, año);
        }

        public async Task<int> ContarDiasComidasMensualAsync(int ninoId, int mes, int año)
        {
            return await _consumoMenuRepository.ContarDiasComidasMensualAsync(ninoId, mes, año);
        }

        public async Task<List<ConsumoMenu>> GenerarReporteConsumoDiarioAsync(DateTime fecha)
        {
            return await ObtenerPorFechaAsync(fecha);
        }

        public async Task<List<ConsumoMenu>> GenerarReporteConsumoMensualAsync(int mes, int año)
        {
            return await _consumoMenuRepository.ObtenerTodosAsync()
                .ContinueWith(task => task.Result
                    .Where(c => c.Fecha.Month == mes && c.Fecha.Year == año)
                    .OrderBy(c => c.Fecha)
                    .ThenBy(c => c.Nino?.Nombre)
                    .ToList());
        }

        public async Task EliminarConsumoAsync(int id)
        {
            var consumo = await _consumoMenuRepository.ObtenerPorIdAsync(id);
            if (consumo == null)
            {
                throw new ArgumentException("El consumo especificado no existe");
            }

            await _consumoMenuRepository.EliminarAsync(id);
        }
    }
}