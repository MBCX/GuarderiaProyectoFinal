using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class MenuPlatoRepository : IMenuPlatoRepository
    {
        private readonly AppDbContext _context;

        public MenuPlatoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuPlato>> ObtenerPorMenuIdAsync(int menuId)
        {
            return await _context.MenuPlatos
                .Include(mp => mp.Plato)
                .Where(mp => mp.Id == menuId)
                .OrderBy(mp => mp.Orden)
                .ToListAsync();
        }

        public async Task<List<MenuPlato>> ObtenerPorPlatoIdAsync(int platoId)
        {
            return await _context.MenuPlatos
                .Include(mp => mp.Menu)
                .Where(mp => mp.PlatoId == platoId)
                .ToListAsync();
        }

        public async Task<MenuPlato> ObtenerRelacionAsync(int menuId, int platoId)
        {
            return await _context.MenuPlatos
                .Include(mp => mp.Menu)
                .Include(mp => mp.Plato)
                .FirstOrDefaultAsync(mp => mp.Id == menuId && mp.PlatoId == platoId);
        }

        public async Task AgregarAsync(MenuPlato menuPlato)
        {
            await _context.MenuPlatos.AddAsync(menuPlato);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(MenuPlato menuPlato)
        {
            _context.MenuPlatos.Update(menuPlato);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int menuId, int platoId)
        {
            var relacion = await _context.MenuPlatos
                .FirstOrDefaultAsync(mp => mp.Id == menuId && mp.PlatoId == platoId);

            if (relacion != null)
            {
                _context.MenuPlatos.Remove(relacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarPorMenuAsync(int menuId)
        {
            var relaciones = await _context.MenuPlatos
                .Where(mp => mp.Id == menuId)
                .ToListAsync();

            _context.MenuPlatos.RemoveRange(relaciones);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Plato>> ObtenerPlatosDeMenuAsync(int menuId)
        {
            return await _context.MenuPlatos
                .Where(mp => mp.Id == menuId)
                .OrderBy(mp => mp.Orden)
                .Select(mp => mp.Plato)
                .ToListAsync();
        }

        public async Task<int> ContarPlatosEnMenuAsync(int menuId)
        {
            return await _context.MenuPlatos
                .CountAsync(mp => mp.Id == menuId);
        }
    }
}
