using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class CargoMensualRepository : ICargoMensualRepository
    {
        private readonly AppDbContext _context;

        public CargoMensualRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CargoMensual> ObtenerPorIdAsync(int id)
        {
            return await _context.CargoMensuales
                .Include(c => c.Nino)
                .Include(c => c.ResponsablePago)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CargoMensual>> ObtenerTodosAsync()
        {
            return await _context.CargoMensuales
                .Include(c => c.Nino)
                .Include(c => c.ResponsablePago)
                .OrderByDescending(c => c.Año)
                .ThenByDescending(c => c.Mes)
                .ToListAsync();
        }

        public async Task<List<CargoMensual>> ObtenerPorNinoIdAsync(int ninoId)
        {
            return await _context.CargoMensuales
                .Include(c => c.Nino)
                .Include(c => c.ResponsablePago)
                .Where(c => c.NinoId == ninoId)
                .OrderByDescending(c => c.Año)
                .ThenByDescending(c => c.Mes)
                .ToListAsync();
        }

        public async Task<List<CargoMensual>> ObtenerPorResponsableAsync(int responsablePagoId)
        {
            return await _context.CargoMensuales
                .Include(c => c.Nino)
                .Include(c => c.ResponsablePago)
                .Where(c => c.ResponsablePagoId == responsablePagoId)
                .OrderByDescending(c => c.Año)
                .ThenByDescending(c => c.Mes)
                .ToListAsync();
        }

        public async Task<CargoMensual> ObtenerPorNinoYMesAsync(int ninoId, int mes, int año)
        {
            return await _context.CargoMensuales
                .Include(c => c.Nino)
                .Include(c => c.ResponsablePago)
                .FirstOrDefaultAsync(c => c.NinoId == ninoId && c.Mes == mes && c.Año == año);
        }

        public async Task<List<CargoMensual>> ObtenerPorMesYAñoAsync(int mes, int año)
        {
            return await _context.CargoMensuales
                .Include(c => c.Nino)
                .Include(c => c.ResponsablePago)
                .Where(c => c.Mes == mes && c.Año == año)
                .OrderBy(c => c.Nino.Nombre)
                .ToListAsync();
        }

        public async Task<List<CargoMensual>> ObtenerPendientesAsync()
        {
            return await _context.CargoMensuales
                .Include(c => c.Nino)
                .Include(c => c.ResponsablePago)
                .Where(c => c.Estado == "Pendiente")
                .OrderBy(c => c.Año)
                .ThenBy(c => c.Mes)
                .ThenBy(c => c.Nino.Nombre)
                .ToListAsync();
        }

        public async Task GenerarAsync(CargoMensual cargoMensual)
        {
            await _context.CargoMensuales.AddAsync(cargoMensual);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(CargoMensual cargoMensual)
        {
            _context.CargoMensuales.Update(cargoMensual);
            await _context.SaveChangesAsync();
        }

        public async Task MarcarComoPagadoAsync(int id, DateTime fechaPago)
        {
            var cargo = await _context.CargoMensuales.FindAsync(id);
            if (cargo != null)
            {
                cargo.FechaPago = fechaPago;
                cargo.Estado = "Pagado";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> ObtenerTotalPendientePorResponsableAsync(int responsablePagoId)
        {
            return await _context.CargoMensuales
                .Where(c => c.ResponsablePagoId == responsablePagoId && c.Estado == "Pendiente")
                .SumAsync(c => c.TotalCargo);
        }
    }
}
