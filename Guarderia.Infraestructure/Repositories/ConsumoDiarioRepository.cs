using Guarderia.Application.Common.Interfaces;
using Guarderia.Domain.Entities;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class ConsumoDiarioRepository : IConsumoDiarioRepository
    {
        private readonly AppDbContext _context;

        public ConsumoDiarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ConsumoDiario>> ObtenerTodosAsync()
        {
            return await _context.ConsumoDiarios
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }

        public async Task<ConsumoDiario?> ObtenerPorIdAsync(int id)
        {
            return await _context.ConsumoDiarios
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<ConsumoDiario>> ObtenerPorNinoIdAsync(int ninoId)
        {
            return await _context.ConsumoDiarios
                .Where(c => c.NinoId == ninoId)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }

        public async Task<List<ConsumoDiario>> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _context.ConsumoDiarios
                .Where(c => c.Fecha.Date == fecha.Date)
                .OrderBy(c => c.NinoId)
                .ToListAsync();
        }

        public async Task<List<ConsumoDiario>> ObtenerPorMesYAnoAsync(int mes, int ano)
        {
            return await _context.ConsumoDiarios
                .Where(c => c.Fecha.Month == mes && c.Fecha.Year == ano)
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.NinoId)
                .ToListAsync();
        }

        public async Task<decimal> CalcularTotalPorNinoYMesAsync(int ninoId, int mes, int ano)
        {
            return await _context.ConsumoDiarios
                .Where(c => c.NinoId == ninoId &&
                           c.Fecha.Month == mes &&
                           c.Fecha.Year == ano)
                .SumAsync(c => c.Monto);
        }

        public async Task AgregarAsync(ConsumoDiario consumo)
        {
            await _context.ConsumoDiarios.AddAsync(consumo);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(ConsumoDiario consumo)
        {
            _context.ConsumoDiarios.Update(consumo);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var consumo = await _context.ConsumoDiarios.FindAsync(id);
            if (consumo != null)
            {
                _context.ConsumoDiarios.Remove(consumo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteRegistroAsync(int ninoId, DateTime fecha)
        {
            return await _context.ConsumoDiarios
                .AnyAsync(c => c.NinoId == ninoId && c.Fecha.Date == fecha.Date);
        }

        public async Task<ConsumoDiario> ObtenerPorNinoYFechaAsync(int ninoId, DateTime fecha)
        {
            return await _context.ConsumoDiarios
                .FirstOrDefaultAsync(c => c.NinoId == ninoId && c.Fecha.Date == fecha.Date);
        }
    }
}
