using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class CostoFijoMensualRepository : ICostoFijoMensualRepository
    {
        private readonly AppDbContext _context;

        public CostoFijoMensualRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CostoFijoMensual> ObtenerPorIdAsync(int id)
        {
            return await _context.CostoFijoMensuals.FindAsync(id);
        }

        public async Task<List<CostoFijoMensual>> ObtenerTodosAsync()
        {
            return await _context.CostoFijoMensuals
                .OrderByDescending(c => c.FechaVigenciaDesde)
                .ToListAsync();
        }

        public async Task<CostoFijoMensual> ObtenerActivoAsync()
        {
            return await _context.CostoFijoMensuals
                .Where(c => c.Activo)
                .OrderByDescending(c => c.FechaVigenciaDesde)
                .FirstOrDefaultAsync();
        }

        public async Task<CostoFijoMensual> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _context.CostoFijoMensuals
                .Where(c => c.FechaVigenciaDesde <= fecha &&
                           (c.FechaVigenciaHasta == null || c.FechaVigenciaHasta >= fecha))
                .OrderByDescending(c => c.FechaVigenciaDesde)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CostoFijoMensual>> ObtenerHistorialAsync()
        {
            return await _context.CostoFijoMensuals
                .OrderByDescending(c => c.FechaVigenciaDesde)
                .ToListAsync();
        }

        public async Task AgregarAsync(CostoFijoMensual costoFijo)
        {
            await _context.CostoFijoMensuals.AddAsync(costoFijo);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(CostoFijoMensual costoFijo)
        {
            _context.CostoFijoMensuals.Update(costoFijo);
            await _context.SaveChangesAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            var costo = await _context.CostoFijoMensuals.FindAsync(id);
            if (costo != null)
            {
                costo.Activo = false;
                costo.FechaVigenciaHasta = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> ObtenerMontoVigenteAsync(DateTime fecha)
        {
            var costoVigente = await ObtenerPorFechaAsync(fecha);
            return costoVigente?.Monto ?? 0;
        }
    }
}
