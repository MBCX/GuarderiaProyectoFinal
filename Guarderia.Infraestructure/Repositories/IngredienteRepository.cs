using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class IngredienteRepository : IIngredienteRepository
    {
        private readonly AppDbContext _context;

        public IngredienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ingrediente>> GetAllAsync()
        {
            return await _context.Ingredientes
                .OrderBy(i => i.Nombre)
                .ToListAsync();
        }

        public async Task<Ingrediente?> GetByNombreAsync(string nombre)
        {
            return await _context.Ingredientes
                .FirstOrDefaultAsync(i => i.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<Ingrediente?> GetByIdAsync(int id)
        {
            return await _context.Ingredientes
                .Include(i => i.Platos)
                .Include(i => i.Alergias)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Ingrediente>> GetAlergenosAsync()
        {
            return await _context.Ingredientes
                .Where(i => i.EsAlergeno)
                .OrderBy(i => i.Nombre)
                .ToListAsync();
        }

        public async Task<List<Ingrediente>> GetNoAlergenosAsync()
        {
            return await _context.Ingredientes
                .Where(i => !i.EsAlergeno)
                .OrderBy(i => i.Nombre)
                .ToListAsync();
        }

        public async Task AddAsync(Ingrediente ingrediente)
        {
            await _context.Ingredientes.AddAsync(ingrediente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ingrediente ingrediente)
        {
            _context.Ingredientes.Update(ingrediente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente != null)
            {
                _context.Ingredientes.Remove(ingrediente);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteAsync(string nombre)
        {
            return await _context.Ingredientes
                .AnyAsync(i => i.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<bool> EsAlergenoAsync(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            return ingrediente?.EsAlergeno ?? false;
        }
    }
}
