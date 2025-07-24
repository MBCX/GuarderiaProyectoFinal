using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class PlatoRepository : IPlatoRepository
    {
        private readonly AppDbContext _context;

        public PlatoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Plato> ObtenerPorIdAsync(int id)
        {
            return await _context.Platos
                .Include(p => p.Ingredientes)
                    .ThenInclude(pi => pi.Ingrediente)
                .Include(p => p.Menus)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Plato> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Platos
                .Include(p => p.Ingredientes)
                    .ThenInclude(pi => pi.Ingrediente)
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<List<Plato>> ObtenerTodosAsync()
        {
            return await _context.Platos
                .Include(p => p.Ingredientes)
                    .ThenInclude(pi => pi.Ingrediente)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<List<Plato>> ObtenerPorMenuIdAsync(int menuId)
        {
            return await _context.Platos
                .Include(p => p.Ingredientes)
                    .ThenInclude(pi => pi.Ingrediente)
                .Where(p => p.Menus.Any(mp => mp.MenuId == menuId))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<List<Plato>> ObtenerPorIngredienteAsync(string ingrediente)
        {
            return await _context.Platos
                .Include(p => p.Ingredientes)
                    .ThenInclude(pi => pi.Ingrediente)
                .Where(p => p.Ingredientes.Any(pi => pi.Ingrediente.Nombre.ToLower().Contains(ingrediente.ToLower())))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task AgregarAsync(Plato plato)
        {
            await _context.Platos.AddAsync(plato);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Plato plato)
        {
            _context.Platos.Update(plato);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var plato = await _context.Platos.FindAsync(id);
            if (plato != null)
            {
                _context.Platos.Remove(plato);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ContieneIngredienteAsync(int platoId, string ingrediente)
        {
            return await _context.Platos
                .Where(p => p.Id == platoId)
                .SelectMany(p => p.Ingredientes)
                .AnyAsync(pi => pi.Ingrediente.Nombre.ToLower().Contains(ingrediente.ToLower()));
        }

        public async Task<List<string>> ObtenerIngredientesDePlatoAsync(int platoId)
        {
            return await _context.Platos
                .Where(p => p.Id == platoId)
                .SelectMany(p => p.Ingredientes)
                .Select(pi => pi.Ingrediente.Nombre)
                .ToListAsync();
        }
    }
}
