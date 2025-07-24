using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class AlergiaRepository : IAlergiaRepository
    {
        private readonly AppDbContext _context;

        public AlergiaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Alergia>> GetAllAsync()
        {
            return await _context.Alergias
                .Include(a => a.Ingrediente)
                .ToListAsync();
        }

        public async Task<Alergia?> GetByIdAsync(int id)
        {
            return await _context.Alergias
                .Include(a => a.Ingrediente)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Alergia>> GetByNinoIdAsync(int ninoId)
        {
            // Nota: Necesitaremos agregar NinoId a la entidad Alergia o crear una relación
            // Por ahora, retornamos las alergias relacionadas con ingredientes que el niño tiene registradas
            return await _context.Alergias
                .Include(a => a.Ingrediente)
                .Where(a => _context.Ninos
                    .Where(n => n.Id == ninoId)
                    .SelectMany(n => n.Alergias)
                    .Any(na => na.Id == a.Id))
                .ToListAsync();
        }

        public async Task<List<string>> GetIngredientesAlergenosPorNinoAsync(int ninoId)
        {
            var nino = await _context.Ninos
                .Include(n => n.Alergias)
                    .ThenInclude(a => a.Ingrediente)
                .FirstOrDefaultAsync(n => n.Id == ninoId);

            if (nino == null || nino.Alergias == null)
                return new List<string>();

            return nino.Alergias
                .Where(a => a.Ingrediente != null)
                .Select(a => a.Ingrediente.Nombre)
                .ToList();
        }

        public async Task<bool> TieneAlergiaAIngredienteAsync(int ninoId, int ingredienteId)
        {
            var nino = await _context.Ninos
                .Include(n => n.Alergias)
                .FirstOrDefaultAsync(n => n.Id == ninoId);

            if (nino == null || nino.Alergias == null)
                return false;

            return nino.Alergias.Any(a => a.Ingrediente != null && a.Ingrediente.Id == ingredienteId);
        }

        public async Task<bool> TieneAlergiaAIngredienteAsync(int ninoId, string nombreIngrediente)
        {
            var nino = await _context.Ninos
                .Include(n => n.Alergias)
                    .ThenInclude(a => a.Ingrediente)
                .FirstOrDefaultAsync(n => n.Id == ninoId);

            if (nino == null || nino.Alergias == null)
                return false;

            return nino.Alergias.Any(a =>
                a.Ingrediente != null &&
                a.Ingrediente.Nombre.Equals(nombreIngrediente, StringComparison.OrdinalIgnoreCase));
        }

        public async Task AddAsync(Alergia alergia)
        {
            await _context.Alergias.AddAsync(alergia);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var alergia = await _context.Alergias.FindAsync(id);
            if (alergia != null)
            {
                _context.Alergias.Remove(alergia);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByNinoEIngredienteAsync(int ninoId, int ingredienteId)
        {
            // Buscar la alergia específica del niño al ingrediente
            var nino = await _context.Ninos
                .Include(n => n.Alergias)
                .FirstOrDefaultAsync(n => n.Id == ninoId);

            if (nino != null && nino.Alergias != null)
            {
                var alergiaAEliminar = nino.Alergias
                    .FirstOrDefault(a => a.Ingrediente != null && a.Ingrediente.Id == ingredienteId);

                if (alergiaAEliminar != null)
                {
                    _context.Alergias.Remove(alergiaAEliminar);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<Nino>> GetNinosConAlergiasAsync()
        {
            return await _context.Ninos
                .Include(n => n.Alergias)
                    .ThenInclude(a => a.Ingrediente)
                .Where(n => n.Alergias.Any())
                .OrderBy(n => n.Nombre)
                .ToListAsync();
        }
    }
}
