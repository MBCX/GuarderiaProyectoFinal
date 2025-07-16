using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IPlatoRepository
    {
        Task<Plato> ObtenerPorIdAsync(int id);
        Task<Plato> ObtenerPorNombreAsync(string nombre);
        Task<List<Plato>> ObtenerTodosAsync();
        Task<List<Plato>> ObtenerPorMenuIdAsync(int menuId);
        Task<List<Plato>> ObtenerPorIngredienteAsync(string ingrediente);
        Task AgregarAsync(Plato plato);
        Task ActualizarAsync(Plato plato);
        Task EliminarAsync(int id);
        Task<bool> ContieneIngredienteAsync(int platoId, string ingrediente);
        Task<List<string>> ObtenerIngredientesDePlatoAsync(int platoId);
    }
}
