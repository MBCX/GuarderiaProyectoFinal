using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IMenuPlatoRepository
    {
        Task<List<MenuPlato>> ObtenerPorMenuIdAsync(int menuId);
        Task<List<MenuPlato>> ObtenerPorPlatoIdAsync(int platoId);
        Task<MenuPlato> ObtenerRelacionAsync(int menuId, int platoId);
        Task AgregarAsync(MenuPlato menuPlato);
        Task ActualizarAsync(MenuPlato menuPlato);
        Task EliminarAsync(int menuId, int platoId);
        Task EliminarPorMenuAsync(int menuId);
        Task<List<Plato>> ObtenerPlatosDeMenuAsync(int menuId);
        Task<int> ContarPlatosEnMenuAsync(int menuId);
    }
}
