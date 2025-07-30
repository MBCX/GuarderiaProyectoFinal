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
    public class IngredienteService : IIngredienteService
    {
        private readonly IIngredienteRepository _ingredienteRepository;
        private readonly IPlatoIngredienteRepository _platoIngredienteRepository;

        public IngredienteService(
            IIngredienteRepository ingredienteRepository,
            IPlatoIngredienteRepository platoIngredienteRepository = null)
        {
            _ingredienteRepository = ingredienteRepository;
            _platoIngredienteRepository = platoIngredienteRepository;
        }

        public async Task<Ingrediente> ObtenerPorIdAsync(int id)
        {
            return await _ingredienteRepository.GetByIdAsync(id);
        }

        public async Task<Ingrediente> ObtenerPorNombreAsync(string nombre)
        {
            return await _ingredienteRepository.GetByNombreAsync(nombre);
        }

        public async Task<List<Ingrediente>> ObtenerTodosAsync()
        {
            return await _ingredienteRepository.GetAllAsync();
        }

        public async Task<List<Ingrediente>> ObtenerAlergenosAsync()
        {
            return await _ingredienteRepository.GetAlergenosAsync();
        }

        public async Task<List<Ingrediente>> ObtenerNoAlergenosAsync()
        {
            return await _ingredienteRepository.GetNoAlergenosAsync();
        }

        public async Task<int> CrearIngredienteAsync(Ingrediente ingrediente)
        {
            // Validaciones
            if (!await ValidarNombreAsync(ingrediente.Nombre))
            {
                throw new ArgumentException("El nombre del ingrediente no es válido");
            }

            if (await ExisteAsync(ingrediente.Nombre))
            {
                throw new InvalidOperationException("Ya existe un ingrediente con ese nombre");
            }

            // Limpiar y normalizar datos
            ingrediente.Nombre = ingrediente.Nombre.Trim();

            if (string.IsNullOrWhiteSpace(ingrediente.Descripcion))
            {
                ingrediente.Descripcion = $"Ingrediente: {ingrediente.Nombre}";
            }

            await _ingredienteRepository.AddAsync(ingrediente);
            return ingrediente.Id;
        }

        public async Task ActualizarIngredienteAsync(Ingrediente ingrediente)
        {
            if (ingrediente == null)
            {
                throw new ArgumentException("El ingrediente no puede ser nulo");
            }

            var ingredienteExistente = await _ingredienteRepository.GetByIdAsync(ingrediente.Id);
            if (ingredienteExistente == null)
            {
                throw new ArgumentException("El ingrediente especificado no existe");
            }

            // Validar nombre si cambio
            if (ingredienteExistente.Nombre.ToLower() != ingrediente.Nombre.ToLower())
            {
                if (!await ValidarNombreAsync(ingrediente.Nombre))
                {
                    throw new ArgumentException("El nombre del ingrediente no es válido");
                }

                if (await ExisteAsync(ingrediente.Nombre))
                {
                    throw new InvalidOperationException("Ya existe un ingrediente con ese nombre");
                }
            }

            // Limpiar datos
            ingrediente.Nombre = ingrediente.Nombre.Trim();

            await _ingredienteRepository.UpdateAsync(ingrediente);
        }

        public async Task MarcarComoAlergenoAsync(int id)
        {
            var ingrediente = await _ingredienteRepository.GetByIdAsync(id);
            if (ingrediente == null)
            {
                throw new ArgumentException("El ingrediente especificado no existe");
            }

            if (!ingrediente.EsAlergeno)
            {
                ingrediente.EsAlergeno = true;
                await _ingredienteRepository.UpdateAsync(ingrediente);
            }
        }

        public async Task DesmarcarComoAlergenoAsync(int id)
        {
            var ingrediente = await _ingredienteRepository.GetByIdAsync(id);
            if (ingrediente == null)
            {
                throw new ArgumentException("El ingrediente especificado no existe");
            }

            if (ingrediente.EsAlergeno)
            {
                ingrediente.EsAlergeno = false;
                await _ingredienteRepository.UpdateAsync(ingrediente);
            }
        }

        public async Task<bool> ExisteAsync(string nombre)
        {
            return await _ingredienteRepository.ExisteAsync(nombre);
        }

        public async Task<bool> EsAlergenoAsync(int id)
        {
            return await _ingredienteRepository.EsAlergenoAsync(id);
        }

        public async Task<bool> ValidarNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return false;

            if (nombre.Trim().Length < 2)
                return false;

            // El nombre no debe contener caracteres especiales peligrosos
            var caracteresProhibidos = new[] {
                '<', '>', '"', '\'',
                '&', ';', '(', ')',
                '{', '}', '[', ']'
            };
            if (nombre.Any(c => caracteresProhibidos.Contains(c)))
                return false;

            await Task.CompletedTask;
            return true;
        }

        public async Task EliminarIngredienteAsync(int id)
        {
            var ingrediente = await _ingredienteRepository.GetByIdAsync(id);
            if (ingrediente == null)
            {
                throw new ArgumentException("El ingrediente especificado no existe");
            }

            if (_platoIngredienteRepository != null)
            {
                var platosQueLoUsan = await _platoIngredienteRepository.ObtenerPorIngredienteIdAsync(id);
                if (platosQueLoUsan.Any())
                {
                    throw new InvalidOperationException(
                        $"No se puede eliminar el ingrediente porque está siendo usado en {platosQueLoUsan.Count} plato(s)");
                }
            }

            await _ingredienteRepository.DeleteAsync(id);
        }

        public async Task<List<Plato>> ObtenerPlatosQueContienenAsync(int ingredienteId)
        {
            if (_platoIngredienteRepository == null)
            {
                return new List<Plato>();
            }

            var relaciones = await _platoIngredienteRepository.ObtenerPorIngredienteIdAsync(ingredienteId);
            return relaciones.Select(r => r.Plato).Where(p => p != null).ToList();
        }
    }
}