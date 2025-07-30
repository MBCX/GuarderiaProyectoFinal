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
    public class ComidaService : IComidaService
    {
        private readonly IComidaRepository _comidaRepository;
        private readonly IAlergiaService _alergiaService;
        private readonly IMenuService _menuService;
        private readonly INinoRepository _ninoRepository;

        public ComidaService(
            IComidaRepository comidaRepository,
            IAlergiaService alergiaService,
            IMenuService menuService = null,
            INinoRepository ninoRepository = null)
        {
            _comidaRepository = comidaRepository;
            _alergiaService = alergiaService;
            _menuService = menuService;
            _ninoRepository = ninoRepository;
        }

        public async Task<List<Comida>> ObtenerComidasPorNiñoAsync(int niñoId)
        {
            return await _comidaRepository.ObtenerPorNinoIdAsync(niñoId);
        }

        public async Task<List<Comida>> ObtenerComidasPorFechaAsync(DateTime fecha)
        {
            return await _comidaRepository.ObtenerPorFechaAsync(fecha);
        }

        public async Task<List<Comida>> ObtenerComidasPorTipoAsync(string tipo)
        {
            return await _comidaRepository.ObtenerPorTipoAsync(tipo);
        }

        public async Task<List<Comida>> ObtenerComidasPorMesAsync(int niñoId, int mes, int año)
        {
            return await _comidaRepository.ObtenerPorMesYAñoAsync(mes, año)
                .ContinueWith(task => task.Result.Where(c => c.NinoId == niñoId).ToList());
        }

        public async Task AsignarComidaAsync(int niñoId, DateTime fecha, string tipoComida, decimal costo)
        {
            if (_ninoRepository != null)
            {
                var nino = await _ninoRepository.ObtenerPorIdAsync(niñoId);
                if (nino == null)
                {
                    throw new ArgumentException("El niño especificado no existe");
                }

                if (!nino.Activo)
                {
                    throw new InvalidOperationException("No se puede asignar comida a un niño inactivo");
                }
            }

            // Validar datos
            if (string.IsNullOrWhiteSpace(tipoComida))
            {
                throw new ArgumentException("El tipo de comida es obligatorio");
            }

            if (costo < 0)
            {
                throw new ArgumentException("El costo no puede ser negativo");
            }

            if (await YaRegistradaAsync(niñoId, fecha, tipoComida))
            {
                throw new InvalidOperationException($"Ya existe un registro de {tipoComida} para el niño en la fecha {fecha:dd/MM/yyyy}");
            }

            var comida = new Comida
            {
                NinoId = niñoId,
                Fecha = fecha,
                Tipo = tipoComida,
                Costo = costo
            };

            await _comidaRepository.RegistrarAsync(comida);
        }

        public async Task ActualizarComidaAsync(Comida comida)
        {
            if (comida == null)
            {
                throw new ArgumentException("La comida no puede ser nula");
            }

            var comidaExistente = await _comidaRepository.ObtenerPorIdAsync(comida.Id);
            if (comidaExistente == null)
            {
                throw new ArgumentException("La comida especificada no existe");
            }

            // Validar datos
            if (string.IsNullOrWhiteSpace(comida.Tipo))
            {
                throw new ArgumentException("El tipo de comida es obligatorio");
            }

            if (comida.Costo < 0)
            {
                throw new ArgumentException("El costo no puede ser negativo");
            }

            await _comidaRepository.ActualizarAsync(comida);
        }

        public async Task<bool> ValidarAlergiaAntesDeComerAsync(int niñoId, string[] ingredientes)
        {
            if (ingredientes == null || !ingredientes.Any())
            {
                return true; // No hay ingredientes que validar
            }

            var alergias = await _alergiaService.ObtenerAlergiasPorNiñoAsync(niñoId);

            foreach (var ingrediente in ingredientes)
            {
                if (alergias.Contains(ingrediente, StringComparer.OrdinalIgnoreCase))
                {
                    // Tiene alergia a al menos un ingrediente
                    return false;
                }
            }

            // No tiene alergias a ningún ingrediente
            return true;
        }

        public async Task<bool> ValidarComidaContraAlergiasAsync(int niñoId, int menuId)
        {
            if (_menuService != null)
            {
                return await _menuService.ValidarMenuParaNiñoAsync(menuId, niñoId);
            }

            // Asumimos que es seguro si no tenemos servicio de menu.
            return true;
        }

        public async Task<decimal> CalcularCostoComidasMensualAsync(int niñoId, int mes, int año)
        {
            return await _comidaRepository.CalcularCostoComidasMensualAsync(niñoId, mes, año);
        }

        public async Task<int> ContarComidasMensualesAsync(int niñoId, int mes, int año)
        {
            return await _comidaRepository.ContarComidasMensualesAsync(niñoId, mes, año);
        }

        public async Task<List<Comida>> GenerarReporteConsumoDiarioAsync(DateTime fecha)
        {
            return await ObtenerComidasPorFechaAsync(fecha);
        }

        public async Task<List<Comida>> GenerarReporteConsumoMensualAsync(int mes, int año)
        {
            return await _comidaRepository.ObtenerPorMesYAñoAsync(mes, año);
        }

        public async Task<bool> YaRegistradaAsync(int niñoId, DateTime fecha, string tipo)
        {
            return await _comidaRepository.YaRegistradaAsync(niñoId, fecha, tipo);
        }

        public async Task EliminarComidaAsync(int id)
        {
            var comida = await _comidaRepository.ObtenerPorIdAsync(id);
            if (comida == null)
            {
                throw new ArgumentException("La comida especificada no existe");
            }

            await _comidaRepository.EliminarAsync(id);
        }

        public async Task<List<string>> ObtenerTiposComidasAsync()
        {
            // Tipos estándar de comidas en una guardería
            return await Task.FromResult(new List<string>
            {
                "Desayuno",
                "Media Mañana",
                "Almuerzo",
                "Merienda",
                "Cena"
            });
        }
    }
}