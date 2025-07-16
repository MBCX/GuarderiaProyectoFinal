using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IMenuRepository
    {
        Task<Menu> ObtenerPorIdAsync(int id);
        Task<List<Menu>> ObtenerTodosAsync();
        Task<List<Menu>> ObtenerActivosAsync();
        Task<List<Menu>> ObtenerPorFechaAsync(DateTime fecha);
        Task<Menu> ObtenerMenuDelDiaAsync(DateTime fecha);
        Task AgregarAsync(Menu menu);
        Task ActualizarAsync(Menu menu);
        Task EliminarAsync(int id);
        Task<bool> TieneIngredienteAlergenoAsync(int menuId, List<string> ingredientesAlergenos);
        Task<List<Menu>> ObtenerMenusSinAlergenosParaNinoAsync(int ninoId);
    }
}
