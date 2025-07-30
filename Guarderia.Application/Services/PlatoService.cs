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
    public class PlatoService : IPlatoService
    {
        private readonly IPlatoRepository _platoRepository;
        private readonly IIngredienteRepository _ingredienteRepository;
        private readonly IPlatoIngredienteRepository _platoIngredienteRepository;
        private readonly IAlergiaRepository _alergiaRepository;

        public PlatoService(
            IPlatoRepository platoRepository,
            IIngredienteRepository ingredienteRepository,
            IPlatoIngredienteRepository platoIngredienteRepository,
            IAlergiaRepository alergiaRepository = null)
        {
            _platoRepository = platoRepository;
            _ingredienteRepository = ingredienteRepository;
            _platoIngredienteRepository = platoIngredienteRepository;
            _alergiaRepository = alergiaRepository;
        }

        public async Task<Plato> ObtenerPorIdAsync(int id)
        {
            return await _platoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<Plato> ObtenerPorNombreAsync(string nombre)
        {
            return await _platoRepository.ObtenerPorNombreAsync(nombre);
        }

        public async Task<List<Plato>> ObtenerTodosAsync()
        {
            return await _platoRepository.ObtenerTodosAsync();
        }

        public async Task<List<Plato>> ObtenerPorMenuAsync(int menuId)
        {
            return await _platoRepository.ObtenerPorMenuIdAsync(menuId);
        }

        public async Task<int> CrearPlatoAsync(Plato plato, List<int> ingredienteIds)
        {
            // Validaciones
            if (plato == null)
            {
                throw new ArgumentException("El plato no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(plato.Nombre))
            {
                throw new ArgumentException("El nombre del plato es obligatorio");
            }

            if (ingredienteIds == null || !ingredienteIds.Any())
            {
                throw new ArgumentException("El plato debe tener al menos un ingrediente");
            }

            var platoExistente = await _platoRepository.ObtenerPorNombreAsync(plato.Nombre);
            if (platoExistente != null)
            {
                throw new InvalidOperationException("Ya existe un plato con ese nombre");
            }

            // Verificar que todos los ingredientes existen
            var ingredientes = new List<Ingrediente>();
            foreach (var ingredienteId in ingredienteIds)
            {
                var ingrediente = await _ingredienteRepository.GetByIdAsync(ingredienteId);
                if (ingrediente == null)
                {
                    throw new ArgumentException($"El ingrediente con ID {ingredienteId} no existe");
                }
                ingredientes.Add(ingrediente);
            }

            // Limpiar datos
            plato.Nombre = plato.Nombre.Trim();
            if (string.IsNullOrWhiteSpace(plato.TipoPlato))
            {
                // Valor por defecto
                plato.TipoPlato = "Principal";
            }

            await _platoRepository.AgregarAsync(plato);

            // Agregar ingredientes al plato
            foreach (var ingredienteId in ingredienteIds)
            {
                await AgregarIngredienteAsync(plato.Id, ingredienteId, "A gusto");
            }

            return plato.Id;
        }

        public async Task ActualizarPlatoAsync(Plato plato)
        {
            if (plato == null)
            {
                throw new ArgumentException("El plato no puede ser nulo");
            }

            var platoExistente = await _platoRepository.ObtenerPorIdAsync(plato.Id);
            if (platoExistente == null)
            {
                throw new ArgumentException("El plato especificado no existe");
            }

            // Validaciones
            if (string.IsNullOrWhiteSpace(plato.Nombre))
            {
                throw new ArgumentException("El nombre del plato es obligatorio");
            }

            // Verificar nombre unico si cambio
            if (platoExistente.Nombre.ToLower() != plato.Nombre.ToLower())
            {
                var platoConMismoNombre = await _platoRepository.ObtenerPorNombreAsync(plato.Nombre);
                if (platoConMismoNombre != null)
                {
                    throw new InvalidOperationException("Ya existe un plato con ese nombre");
                }
            }

            // Limpiar datos
            plato.Nombre = plato.Nombre.Trim();

            await _platoRepository.ActualizarAsync(plato);
        }

        public async Task AgregarIngredienteAsync(int platoId, int ingredienteId, string cantidad)
        {
            var plato = await _platoRepository.ObtenerPorIdAsync(platoId);
            if (plato == null)
            {
                throw new ArgumentException("El plato especificado no existe");
            }

            var ingrediente = await _ingredienteRepository.GetByIdAsync(ingredienteId);
            if (ingrediente == null)
            {
                throw new ArgumentException("El ingrediente especificado no existe");
            }

            if (await _platoIngredienteRepository.PlatoContieneIngredienteAsync(platoId, ingredienteId))
            {
                throw new InvalidOperationException("El ingrediente ya está incluido en el plato");
            }

            var platoIngrediente = new PlatoIngrediente
            {
                PlatoId = platoId,
                IngredienteId = ingredienteId,
                Cantidad = cantidad ?? "A gusto",
                EsAlergeno = ingrediente.EsAlergeno
            };

            await _platoIngredienteRepository.AgregarAsync(platoIngrediente);
        }

        public async Task RemoverIngredienteAsync(int platoId, int ingredienteId)
        {
            var relacion = await _platoIngredienteRepository.ObtenerRelacionAsync(platoId, ingredienteId);
            if (relacion == null)
            {
                throw new ArgumentException("El ingrediente no está incluido en el plato");
            }

            var ingredientesDelPlato = await _platoIngredienteRepository.ObtenerPorPlatoIdAsync(platoId);
            if (ingredientesDelPlato.Count <= 1)
            {
                throw new InvalidOperationException("No se puede eliminar el último ingrediente del plato");
            }

            await _platoIngredienteRepository.EliminarAsync(platoId, ingredienteId);
        }

        public async Task<List<Ingrediente>> ObtenerIngredientesAsync(int platoId)
        {
            return await _platoIngredienteRepository.ObtenerIngredientesDePlatoAsync(platoId);
        }

        public async Task<bool> ContieneIngredienteAsync(int platoId, string nombreIngrediente)
        {
            return await _platoRepository.ContieneIngredienteAsync(platoId, nombreIngrediente);
        }

        public async Task<bool> EsAptoParaNiñoAsync(int platoId, int ninoId)
        {
            if (_alergiaRepository == null)
            {
                // Sin acceso a alergias, asumimos que es apto
                return true;
            }

            var ingredientesDelPlato = await _platoIngredienteRepository.ObtenerIngredientesDePlatoAsync(platoId);

            foreach (var ingrediente in ingredientesDelPlato)
            {
                if (await _alergiaRepository.TieneAlergiaAIngredienteAsync(ninoId, ingrediente.Id))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<List<string>> ObtenerAlergenosEnPlatoAsync(int platoId, int ninoId)
        {
            var alergenosEncontrados = new List<string>();

            if (_alergiaRepository == null)
            {
                return alergenosEncontrados;
            }

            var ingredientesDelPlato = await _platoIngredienteRepository.ObtenerIngredientesDePlatoAsync(platoId);

            foreach (var ingrediente in ingredientesDelPlato)
            {
                if (await _alergiaRepository.TieneAlergiaAIngredienteAsync(ninoId, ingrediente.Id))
                {
                    alergenosEncontrados.Add(ingrediente.Nombre);
                }
            }

            return alergenosEncontrados;
        }

        public async Task EliminarPlatoAsync(int id)
        {
            var plato = await _platoRepository.ObtenerPorIdAsync(id);
            if (plato == null)
            {
                throw new ArgumentException("El plato especificado no existe");
            }

            // Eliminar primero todas las relaciones con ingredientes
            await _platoIngredienteRepository.EliminarPorPlatoAsync(id);

            // Luego eliminar el plato
            await _platoRepository.EliminarAsync(id);
        }
    }
}