using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class ComidaRepository : IComidaRepository
    {
        private readonly AppDbContext _context;

        public ComidaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Comida> ObtenerPorIdAsync(int id)
        {
            return await _context.Comidas
                .Include(c => c.Nino)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Comida>> ObtenerTodasAsync()
        {
            return await _context.Comidas
                .Include(c => c.Nino)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }

        public async Task<List<Comida>> ObtenerPorNinoIdAsync(int ninoId)
        {
            return await _context.Comidas
                .Include(c => c.Nino)
                .Where(c => c.NinoId == ninoId)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }

        public async Task<List<Comida>> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _context.Comidas
                .Include(c => c.Nino)
                .Where(c => c.Fecha.Date == fecha.Date)
                .OrderBy(c => c.Nino.Nombre)
                .ThenBy(c => c.Tipo)
                .ToListAsync();
        }

        public async Task<List<Comida>> ObtenerPorTipoAsync(string tipo)
        {
            return await _context.Comidas
                .Include(c => c.Nino)
                .Where(c => c.Tipo.ToLower() == tipo.ToLower())
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }

        public async Task<List<Comida>> ObtenerPorMesYAñoAsync(int mes, int año)
        {
            return await _context.Comidas
                .Include(c => c.Nino)
                .Where(c => c.Fecha.Month == mes && c.Fecha.Year == año)
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Nino.Nombre)
                .ToListAsync();
        }

        public async Task<decimal> CalcularCostoComidasMensualAsync(int ninoId, int mes, int año)
        {
            return await _context.Comidas
                .Where(c => c.NinoId == ninoId &&
                           c.Fecha.Month == mes &&
                           c.Fecha.Year == año)
                .SumAsync(c => c.Costo);
        }

        public async Task RegistrarAsync(Comida comida)
        {
            await _context.Comidas.AddAsync(comida);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Comida comida)
        {
            _context.Comidas.Update(comida);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var comida = await _context.Comidas.FindAsync(id);
            if (comida != null)
            {
                _context.Comidas.Remove(comida);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> YaRegistradaAsync(int ninoId, DateTime fecha, string tipo)
        {
            return await _context.Comidas
                .AnyAsync(c => c.NinoId == ninoId &&
                              c.Fecha.Date == fecha.Date &&
                              c.Tipo.ToLower() == tipo.ToLower());
        }

        public async Task<int> ContarComidasMensualesAsync(int ninoId, int mes, int año)
        {
            return await _context.Comidas
                .CountAsync(c => c.NinoId == ninoId &&
                                c.Fecha.Month == mes &&
                                c.Fecha.Year == año);
        }
    }
}
