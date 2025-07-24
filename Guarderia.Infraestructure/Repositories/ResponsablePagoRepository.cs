using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class ResponsablePagoRepository : IResponsablePagoRepository
    {
        private readonly AppDbContext _context;

        public ResponsablePagoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponsablePago> ObtenerPorIdAsync(int id)
        {
            return await _context.ResponsablePagos
                .Include(r => r.NinosAPagar)
                .Include(r => r.NinosAutorizados)
                    .ThenInclude(na => na.Nino)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ResponsablePago> ObtenerPorCedulaAsync(string cedula)
        {
            return await _context.ResponsablePagos
                .Include(r => r.NinosAPagar)
                .Include(r => r.NinosAutorizados)
                    .ThenInclude(na => na.Nino)
                .FirstOrDefaultAsync(r => r.Cedula == cedula);
        }

        public async Task<List<ResponsablePago>> ObtenerTodosAsync()
        {
            return await _context.ResponsablePagos
                .Include(r => r.NinosAPagar)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
        }

        public async Task<List<ResponsablePago>> ObtenerPorNinoIdAsync(int ninoId)
        {
            return await _context.ResponsablePagos
                .Include(r => r.NinosAPagar)
                .Where(r => r.NinosAPagar.Any(n => n.Id == ninoId))
                .ToListAsync();
        }

        public async Task AgregarAsync(ResponsablePago responsablePago)
        {
            await _context.ResponsablePagos.AddAsync(responsablePago);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(ResponsablePago responsablePago)
        {
            _context.ResponsablePagos.Update(responsablePago);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var responsable = await _context.ResponsablePagos.FindAsync(id);
            if (responsable != null)
            {
                _context.ResponsablePagos.Remove(responsable);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistePorCedulaAsync(string cedula)
        {
            return await _context.ResponsablePagos
                .AnyAsync(r => r.Cedula == cedula);
        }

        public async Task<bool> TieneCuentaCorrienteValidaAsync(int id)
        {
            var responsable = await _context.ResponsablePagos.FindAsync(id);
            return responsable != null && !string.IsNullOrWhiteSpace(responsable.CuentaCorriente);
        }
    }
}
