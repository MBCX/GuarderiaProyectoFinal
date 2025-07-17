using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IMenuService
    {
        Task<Menu> ObtenerPorIdAsync(int id);
        Task<List<Menu>> ObtenerTodosAsync();
        Task<List<Menu>> ObtenerActivosAsync();
        Task<Menu> ObtenerMenuDelDiaAsync(DateTime fecha);
        Task<List<Menu>> ObtenerMenusSinAlergenosParaNiñoAsync(int ninoId);
        Task<int> CrearMenuAsync(Menu menu, List<int> platoIds);
        Task ActualizarMenuAsync(Menu menu);
        Task AgregarPlatoAMenuAsync(int menuId, int platoId, int orden);
        Task RemoverPlatoDeMenuAsync(int menuId, int platoId);
        Task<bool> MenuTieneIngredienteAlergenoAsync(int menuId, int ninoId);
        Task<List<string>> ObtenerIngredientesDeMenuAsync(int menuId);
        Task<bool> ValidarMenuParaNiñoAsync(int menuId, int ninoId);
        Task ActivarMenuAsync(int id);
        Task DesactivarMenuAsync(int id);
    }
}
