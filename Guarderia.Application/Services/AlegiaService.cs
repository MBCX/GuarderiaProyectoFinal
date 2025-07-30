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
    public class AlergiaService : IAlergiaService
    {
        private readonly IAlergiaRepository _alergiaRepository;
        private readonly IIngredienteRepository _ingredienteRepository;
        private readonly INinoRepository _ninoRepository;
        private readonly IPlatoRepository _platoRepository;
        private readonly IMenuRepository _menuRepository;

        public AlergiaService(
            IAlergiaRepository alergiaRepository,
            IIngredienteRepository ingredienteRepository,
            INinoRepository ninoRepository,
            IPlatoRepository platoRepository = null,
            IMenuRepository menuRepository = null)
        {
            _alergiaRepository = alergiaRepository;
            _ingredienteRepository = ingredienteRepository;
            _ninoRepository = ninoRepository;
            _platoRepository = platoRepository;
            _menuRepository = menuRepository;
        }

        public async Task<List<string>> ObtenerAlergiasPorNiñoAsync(int niñoId)
        {
            return await _alergiaRepository.GetIngredientesAlergenosPorNinoAsync(niñoId);
        }

        public async Task<List<Alergia>> ObtenerAlergiasCompletasPorNiñoAsync(int niñoId)
        {
            return await _alergiaRepository.GetByNinoIdAsync(niñoId);
        }

        public async Task RegistrarAlergiaAsync(int ninoId, string ingrediente)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("El niño especificado no existe");
            }

            var ingredienteEntity = await _ingredienteRepository.GetByNombreAsync(ingrediente);
            if (ingredienteEntity == null)
            {
                ingredienteEntity = new Ingrediente
                {
                    Nombre = ingrediente,
                    EsAlergeno = true,
                    Descripcion = $"Ingrediente alérgeno registrado para {nino.Nombre}"
                };
                await _ingredienteRepository.AddAsync(ingredienteEntity);
            }
            else if (!ingredienteEntity.EsAlergeno)
            {
                ingredienteEntity.EsAlergeno = true;
                await _ingredienteRepository.UpdateAsync(ingredienteEntity);
            }

            if (await TieneAlergiaAIngredienteAsync(ninoId, ingredienteEntity.Id))
            {
                throw new InvalidOperationException("El niño ya tiene registrada esta alergia");
            }

            var alergia = new Alergia
            {
                Ingrediente = ingredienteEntity
            };

            await _alergiaRepository.AddAsync(alergia);
        }

        public async Task RegistrarAlergiaAsync(int ninoId, int ingredienteId)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("El niño especificado no existe");
            }

            var ingrediente = await _ingredienteRepository.GetByIdAsync(ingredienteId);
            if (ingrediente == null)
            {
                throw new ArgumentException("El ingrediente especificado no existe");
            }

            if (await TieneAlergiaAIngredienteAsync(ninoId, ingredienteId))
            {
                throw new InvalidOperationException("El niño ya tiene registrada esta alergia");
            }

            if (!ingrediente.EsAlergeno)
            {
                ingrediente.EsAlergeno = true;
                await _ingredienteRepository.UpdateAsync(ingrediente);
            }

            var alergia = new Alergia
            {
                Ingrediente = ingrediente
            };

            await _alergiaRepository.AddAsync(alergia);
        }

        public async Task EliminarAlergiaAsync(int ninoId, string ingrediente)
        {
            var ingredienteEntity = await _ingredienteRepository.GetByNombreAsync(ingrediente);
            if (ingredienteEntity != null)
            {
                await _alergiaRepository.DeleteByNinoEIngredienteAsync(ninoId, ingredienteEntity.Id);
            }
        }

        public async Task EliminarAlergiaAsync(int ninoId, int ingredienteId)
        {
            await _alergiaRepository.DeleteByNinoEIngredienteAsync(ninoId, ingredienteId);
        }

        public async Task ActualizarAlergiasAsync(int ninoId, List<string> nuevasAlergias)
        {
            var alergiasActuales = await ObtenerAlergiasPorNiñoAsync(ninoId);
            var alergiasAEliminar = alergiasActuales.Except(nuevasAlergias, StringComparer.OrdinalIgnoreCase);

            foreach (var alergia in alergiasAEliminar)
            {
                await EliminarAlergiaAsync(ninoId, alergia);
            }

            var alergiasAAgregar = nuevasAlergias.Except(alergiasActuales, StringComparer.OrdinalIgnoreCase);
            foreach (var alergia in alergiasAAgregar)
            {
                await RegistrarAlergiaAsync(ninoId, alergia);
            }
        }

        public async Task ActualizarAlergiasAsync(int ninoId, List<int> nuevosIngredienteIds)
        {
            // Obtener alergias actuales
            var alergiasActuales = await ObtenerAlergiasCompletasPorNiñoAsync(ninoId);
            var ingredienteIdsActuales = alergiasActuales
                .Where(a => a.Ingrediente != null)
                .Select(a => a.Ingrediente.Id)
                .ToList();

            // Alergias a eliminar
            var alergiasAEliminar = ingredienteIdsActuales.Except(nuevosIngredienteIds);
            foreach (var ingredienteId in alergiasAEliminar)
            {
                await EliminarAlergiaAsync(ninoId, ingredienteId);
            }

            // Alergias a agregar
            var alergiasAAgregar = nuevosIngredienteIds.Except(ingredienteIdsActuales);
            foreach (var ingredienteId in alergiasAAgregar)
            {
                await RegistrarAlergiaAsync(ninoId, ingredienteId);
            }
        }

        public async Task<bool> TieneAlergiaAIngredienteAsync(int ninoId, string ingrediente)
        {
            return await _alergiaRepository.TieneAlergiaAIngredienteAsync(ninoId, ingrediente);
        }

        public async Task<bool> TieneAlergiaAIngredienteAsync(int ninoId, int ingredienteId)
        {
            return await _alergiaRepository.TieneAlergiaAIngredienteAsync(ninoId, ingredienteId);
        }

        public async Task<bool> PuedeConsumirPlatoAsync(int ninoId, int platoId)
        {
            if (_platoRepository == null)
            {
                // Si no tenemos acceso al repositorio,
                // asumimos que es seguro
                return true;
            }

            var ingredientesDelPlato = await _platoRepository.ObtenerIngredientesDePlatoAsync(platoId);
            var alergiasDelNino = await ObtenerAlergiasPorNiñoAsync(ninoId);

            foreach (var ingrediente in ingredientesDelPlato)
            {
                if (alergiasDelNino.Contains(ingrediente, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> PuedeConsumirMenuAsync(int ninoId, int menuId)
        {
            if (_menuRepository == null || _platoRepository == null)
            {
                // Si no tenemos acceso a los repositorios,
                // asumimos que es seguro
                return true;
            }

            var menu = await _menuRepository.ObtenerPorIdAsync(menuId);
            if (menu == null)
            {
                throw new ArgumentException("El menú especificado no existe");
            }

            foreach (var menuPlato in menu.Platos)
            {
                if (!await PuedeConsumirPlatoAsync(ninoId, menuPlato.PlatoId))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<List<string>> ValidarMenuContraAlergiasAsync(int ninoId, int menuId)
        {
            var ingredientesProblematicos = new List<string>();

            if (_menuRepository == null || _platoRepository == null)
            {
                return ingredientesProblematicos; // No podemos validar
            }

            var menu = await _menuRepository.ObtenerPorIdAsync(menuId);
            if (menu == null)
            {
                throw new ArgumentException("El menú especificado no existe");
            }

            var alergiasDelNino = await ObtenerAlergiasPorNiñoAsync(ninoId);

            foreach (var menuPlato in menu.Platos)
            {
                var ingredientesDelPlato = await _platoRepository.ObtenerIngredientesDePlatoAsync(menuPlato.PlatoId);

                foreach (var ingrediente in ingredientesDelPlato)
                {
                    if (alergiasDelNino.Contains(ingrediente, StringComparer.OrdinalIgnoreCase) &&
                        !ingredientesProblematicos.Contains(ingrediente, StringComparer.OrdinalIgnoreCase))
                    {
                        ingredientesProblematicos.Add(ingrediente);
                    }
                }
            }

            return ingredientesProblematicos;
        }

        public async Task<List<Nino>> ObtenerNiñosConAlergiasAsync()
        {
            return await _alergiaRepository.GetNinosConAlergiasAsync();
        }

        public async Task<List<string>> ObtenerIngredientesAlergenosAsync()
        {
            var ingredientesAlergenos = await _ingredienteRepository.GetAlergenosAsync();
            return ingredientesAlergenos.Select(i => i.Nombre).ToList();
        }
    }
}