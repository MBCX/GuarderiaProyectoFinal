using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class NinoRepository : INinoRepository
    {
        private readonly AppDbContext _context;

        public NinoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Nino> ObtenerPorIdAsync(int id)
        {
            return await _context.Ninos
                .Include(n => n.ResponsablePago)
                .Include(n => n.PersonasAutorizadas)
                    .ThenInclude(pa => pa.PersonaAutorizada)
                .Include(n => n.Asistencias)
                .Include(n => n.Comidas)
                .Include(n => n.Alergias)
                    .ThenInclude(a => a.Ingrediente)
                .Include(n => n.ConsumosMenu)
                .Include(n => n.CargosMensuales)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Nino> ObtenerPorMatriculaAsync(string numeroMatricula)
        {
            return await _context.Ninos
                .Include(n => n.ResponsablePago)
                .Include(n => n.PersonasAutorizadas)
                    .ThenInclude(pa => pa.PersonaAutorizada)
                .FirstOrDefaultAsync(n => n.NumeroMatricula == numeroMatricula);
        }

        public async Task<List<Nino>> ObtenerTodosAsync()
        {
            return await _context.Ninos
                .Include(n => n.ResponsablePago)
                .OrderBy(n => n.Nombre)
                .ToListAsync();
        }

        public async Task<List<Nino>> ObtenerActivosAsync()
        {
            return await _context.Ninos
                .Include(n => n.ResponsablePago)
                .Where(n => n.Activo)
                .OrderBy(n => n.Nombre)
                .ToListAsync();
        }

        public async Task<List<Nino>> ObtenerInactivosAsync()
        {
            return await _context.Ninos
                .Include(n => n.ResponsablePago)
                .Where(n => !n.Activo)
                .OrderBy(n => n.Nombre)
                .ToListAsync();
        }

        public async Task<List<Nino>> ObtenerPorResponsablePagoAsync(int responsablePagoId)
        {
            return await _context.Ninos
                .Where(n => n.ResponsablePagoId == responsablePagoId)
                .OrderBy(n => n.Nombre)
                .ToListAsync();
        }

        public async Task AgregarAsync(Nino nino)
        {
            await _context.Ninos.AddAsync(nino);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Nino nino)
        {
            _context.Ninos.Update(nino);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var nino = await _context.Ninos.FindAsync(id);
            if (nino != null)
            {
                _context.Ninos.Remove(nino);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DarBajaAsync(int id, DateTime fechaBaja)
        {
            var nino = await _context.Ninos.FindAsync(id);
            if (nino != null)
            {
                nino.FechaBaja = fechaBaja;
                nino.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReactivarAsync(int id)
        {
            var nino = await _context.Ninos.FindAsync(id);
            if (nino != null)
            {
                nino.FechaBaja = null;
                nino.Activo = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteMatriculaAsync(string numeroMatricula)
        {
            return await _context.Ninos
                .AnyAsync(n => n.NumeroMatricula == numeroMatricula);
        }

        public async Task<int> ContarActivosAsync()
        {
            return await _context.Ninos.CountAsync(n => n.Activo);
        }
    }
}
