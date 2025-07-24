using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class ConsumoMenuRepository : IConsumoMenuRepository
    {
        private readonly AppDbContext _context;

        public ConsumoMenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ConsumoMenu> ObtenerPorIdAsync(int id)
        {
            return await _context.ConsumosMenu
                .Include(c => c.Nino)
                .Include(c => c.Menu)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<ConsumoMenu>> ObtenerTodosAsync()
        {
            return await _context.ConsumosMenu
                .Include(c => c.Nino)
                .Include(c => c.Menu)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }

        public async Task<List<ConsumoMenu>> ObtenerPorNinoIdAsync(int ninoId)
        {
            return await _context.ConsumosMenu
                .Include(c => c.Menu)
                .Where(c => c.NinoId == ninoId)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }

        public async Task<List<ConsumoMenu>> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _context.ConsumosMenu
                .Include(c => c.Nino)
                .Include(c => c.Menu)
                .Where(c => c.Fecha.Date == fecha.Date)
                .OrderBy(c => c.Nino.Nombre)
                .ToListAsync();
        }

        public async Task<List<ConsumoMenu>> ObtenerPorNinoYMesAsync(int ninoId, int mes, int año)
        {
            return await _context.ConsumosMenu
                .Include(c => c.Menu)
                .Where(c => c.NinoId == ninoId && c.Fecha.Month == mes && c.Fecha.Year == año)
                .OrderBy(c => c.Fecha)
                .ToListAsync();
        }

        public async Task<ConsumoMenu> ObtenerPorNinoYFechaAsync(int ninoId, DateTime fecha)
        {
            return await _context.ConsumosMenu
                .Include(c => c.Menu)
                .FirstOrDefaultAsync(c => c.NinoId == ninoId && c.Fecha.Date == fecha.Date);
        }

        public async Task RegistrarAsync(ConsumoMenu consumoMenu)
        {
            await _context.ConsumosMenu.AddAsync(consumoMenu);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(ConsumoMenu consumoMenu)
        {
            _context.ConsumosMenu.Update(consumoMenu);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var consumo = await _context.ConsumosMenu.FindAsync(id);
            if (consumo != null)
            {
                _context.ConsumosMenu.Remove(consumo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> CalcularCostoMenusMensualAsync(int ninoId, int mes, int año)
        {
            return await _context.ConsumosMenu
                .Where(c => c.NinoId == ninoId && c.Fecha.Month == mes && c.Fecha.Year == año)
                .SumAsync(c => c.CostoReal);
        }

        public async Task<int> ContarDiasComidasMensualAsync(int ninoId, int mes, int año)
        {
            return await _context.ConsumosMenu
                .CountAsync(c => c.NinoId == ninoId && c.Fecha.Month == mes && c.Fecha.Year == año);
        }
    }
}
