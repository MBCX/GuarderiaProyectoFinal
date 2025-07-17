using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IPlatoService
    {
        Task<Plato> ObtenerPorIdAsync(int id);
        Task<Plato> ObtenerPorNombreAsync(string nombre);
        Task<List<Plato>> ObtenerTodosAsync();
        Task<List<Plato>> ObtenerPorMenuAsync(int menuId);
        Task<int> CrearPlatoAsync(Plato plato, List<int> ingredienteIds);
        Task ActualizarPlatoAsync(Plato plato);
        Task AgregarIngredienteAsync(int platoId, int ingredienteId, string cantidad);
        Task RemoverIngredienteAsync(int platoId, int ingredienteId);
        Task<List<Ingrediente>> ObtenerIngredientesAsync(int platoId);
        Task<bool> ContieneIngredienteAsync(int platoId, string nombreIngrediente);
        Task<bool> EsAptoParaNiñoAsync(int platoId, int ninoId);
        Task<List<string>> ObtenerAlergenosEnPlatoAsync(int platoId, int ninoId);
        Task EliminarPlatoAsync(int id);
    }
}
