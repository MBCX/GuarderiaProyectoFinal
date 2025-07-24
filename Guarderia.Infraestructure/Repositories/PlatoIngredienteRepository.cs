using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class PlatoIngredienteRepository : IPlatoIngredienteRepository
    {
        private readonly AppDbContext _context;

        public PlatoIngredienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlatoIngrediente>> ObtenerPorPlatoIdAsync(int platoId)
        {
            return await _context.PlatoIngredientes
                .Include(pi => pi.Ingrediente)
                .Where(pi => pi.PlatoId == platoId)
                .ToListAsync();
        }

        public async Task<List<PlatoIngrediente>> ObtenerPorIngredienteIdAsync(int ingredienteId)
        {
            return await _context.PlatoIngredientes
                .Include(pi => pi.Plato)
                .Where(pi => pi.IngredienteId == ingredienteId)
                .ToListAsync();
        }

        public async Task<PlatoIngrediente> ObtenerRelacionAsync(int platoId, int ingredienteId)
        {
            return await _context.PlatoIngredientes
                .Include(pi => pi.Plato)
                .Include(pi => pi.Ingrediente)
                .FirstOrDefaultAsync(pi => pi.PlatoId == platoId && pi.IngredienteId == ingredienteId);
        }

        public async Task AgregarAsync(PlatoIngrediente platoIngrediente)
        {
            await _context.PlatoIngredientes.AddAsync(platoIngrediente);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(PlatoIngrediente platoIngrediente)
        {
            _context.PlatoIngredientes.Update(platoIngrediente);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int platoId, int ingredienteId)
        {
            var relacion = await _context.PlatoIngredientes
                .FirstOrDefaultAsync(pi => pi.PlatoId == platoId && pi.IngredienteId == ingredienteId);

            if (relacion != null)
            {
                _context.PlatoIngredientes.Remove(relacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarPorPlatoAsync(int platoId)
        {
            var relaciones = await _context.PlatoIngredientes
                .Where(pi => pi.PlatoId == platoId)
                .ToListAsync();

            _context.PlatoIngredientes.RemoveRange(relaciones);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Ingrediente>> ObtenerIngredientesDePlatoAsync(int platoId)
        {
            return await _context.PlatoIngredientes
                .Where(pi => pi.PlatoId == platoId)
                .Select(pi => pi.Ingrediente)
                .ToListAsync();
        }

        public async Task<List<string>> ObtenerNombresIngredientesDePlatoAsync(int platoId)
        {
            return await _context.PlatoIngredientes
                .Where(pi => pi.PlatoId == platoId)
                .Select(pi => pi.Ingrediente.Nombre)
                .ToListAsync();
        }

        public async Task<bool> PlatoContieneIngredienteAsync(int platoId, int ingredienteId)
        {
            return await _context.PlatoIngredientes
                .AnyAsync(pi => pi.PlatoId == platoId && pi.IngredienteId == ingredienteId);
        }
    }
}
