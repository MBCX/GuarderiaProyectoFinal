using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Menu> ObtenerPorIdAsync(int id)
        {
            return await _context.Menus
                .Include(m => m.Platos)
                    .ThenInclude(mp => mp.Plato)
                        .ThenInclude(p => p.Ingredientes)
                            .ThenInclude(pi => pi.Ingrediente)
                .Include(m => m.Consumos)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Menu>> ObtenerTodosAsync()
        {
            return await _context.Menus
                .Include(m => m.Platos)
                    .ThenInclude(mp => mp.Plato)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<List<Menu>> ObtenerActivosAsync()
        {
            return await _context.Menus
                .Include(m => m.Platos)
                    .ThenInclude(mp => mp.Plato)
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<List<Menu>> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _context.Menus
                .Include(m => m.Platos)
                    .ThenInclude(mp => mp.Plato)
                .Include(m => m.Consumos.Where(c => c.Fecha.Date == fecha.Date))
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<Menu> ObtenerMenuDelDiaAsync(DateTime fecha)
        {
            return await _context.Menus
                .Include(m => m.Platos)
                    .ThenInclude(mp => mp.Plato)
                .Include(m => m.Consumos.Where(c => c.Fecha.Date == fecha.Date))
                .Where(m => m.Activo && m.Consumos.Any(c => c.Fecha.Date == fecha.Date))
                .OrderByDescending(m => m.Consumos.Count(c => c.Fecha.Date == fecha.Date))
                .FirstOrDefaultAsync();
        }

        public async Task AgregarAsync(Menu menu)
        {
            await _context.Menus.AddAsync(menu);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Menu menu)
        {
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TieneIngredienteAlergenoAsync(int menuId, List<string> ingredientesAlergenos)
        {
            var menu = await _context.Menus
                .Include(m => m.Platos)
                    .ThenInclude(mp => mp.Plato)
                        .ThenInclude(p => p.Ingredientes)
                            .ThenInclude(pi => pi.Ingrediente)
                .FirstOrDefaultAsync(m => m.Id == menuId);

            if (menu == null) return false;

            var ingredientesDelMenu = menu.Platos
                .SelectMany(mp => mp.Plato.Ingredientes)
                .Select(pi => pi.Ingrediente.Nombre.ToLower())
                .ToList();

            return ingredientesAlergenos
                .Any(alergia => ingredientesDelMenu.Contains(alergia.ToLower()));
        }

        public async Task<List<Menu>> ObtenerMenusSinAlergenosParaNinoAsync(int ninoId)
        {
            var nino = await _context.Ninos
                .Include(n => n.Alergias)
                    .ThenInclude(a => a.Ingrediente)
                .FirstOrDefaultAsync(n => n.Id == ninoId);

            if (nino == null || nino.Alergias == null || !nino.Alergias.Any())
            {
                return await ObtenerActivosAsync();
            }

            var ingredientesAlergenos = nino.Alergias
                .Where(a => a.Ingrediente != null)
                .Select(a => a.Ingrediente.Nombre.ToLower())
                .ToList();

            var menusDisponibles = new List<Menu>();
            var todosLosMenus = await ObtenerActivosAsync();

            foreach (var menu in todosLosMenus)
            {
                var menuCompleto = await ObtenerPorIdAsync(menu.Id);

                var ingredientesDelMenu = menuCompleto.Platos
                    .SelectMany(mp => mp.Plato.Ingredientes)
                    .Select(pi => pi.Ingrediente.Nombre.ToLower())
                    .ToList();

                bool tieneAlergeno = ingredientesAlergenos
                    .Any(alergia => ingredientesDelMenu.Contains(alergia));

                if (!tieneAlergeno)
                {
                    menusDisponibles.Add(menu);
                }
            }

            return menusDisponibles;
        }
    }
}
