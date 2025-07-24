using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class AsistenciaRepository : IAsistenciaRepository
    {
        private readonly AppDbContext _context;

        public AsistenciaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Asistencia> ObtenerPorIdAsync(int id)
        {
            return await _context.Asistencias
                .Include(a => a.Nino)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Asistencia>> ObtenerTodasAsync()
        {
            return await _context.Asistencias
                .Include(a => a.Nino)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<List<Asistencia>> ObtenerPorNinoIdAsync(int ninoId)
        {
            return await _context.Asistencias
                .Include(a => a.Nino)
                .Where(a => a.NinoId == ninoId)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<List<Asistencia>> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _context.Asistencias
                .Include(a => a.Nino)
                .Where(a => a.Fecha.Date == fecha.Date)
                .OrderBy(a => a.Nino.Nombre)
                .ToListAsync();
        }

        public async Task<List<Asistencia>> ObtenerPorMesYAñoAsync(int mes, int año)
        {
            return await _context.Asistencias
                .Include(a => a.Nino)
                .Where(a => a.Fecha.Month == mes && a.Fecha.Year == año)
                .OrderBy(a => a.Fecha)
                .ThenBy(a => a.Nino.Nombre)
                .ToListAsync();
        }

        public async Task<int> ContarAsistenciasMensualesAsync(int ninoId, int mes, int año)
        {
            return await _context.Asistencias
                .CountAsync(a => a.NinoId == ninoId &&
                                a.Fecha.Month == mes &&
                                a.Fecha.Year == año &&
                                a.Asistio);
        }

        public async Task RegistrarAsync(Asistencia asistencia)
        {
            await _context.Asistencias.AddAsync(asistencia);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Asistencia asistencia)
        {
            _context.Asistencias.Update(asistencia);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var asistencia = await _context.Asistencias.FindAsync(id);
            if (asistencia != null)
            {
                _context.Asistencias.Remove(asistencia);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> YaRegistradaAsync(int ninoId, DateTime fecha)
        {
            return await _context.Asistencias
                .AnyAsync(a => a.NinoId == ninoId && a.Fecha.Date == fecha.Date);
        }

        public async Task<decimal> CalcularPorcentajeAsistenciaAsync(int ninoId, int mes, int año)
        {
            var totalAsistencias = await _context.Asistencias
                .CountAsync(a => a.NinoId == ninoId &&
                                a.Fecha.Month == mes &&
                                a.Fecha.Year == año);

            if (totalAsistencias == 0)
            {
                return 0;
            }

            var asistenciasPositivas = await _context.Asistencias
                .CountAsync(a => a.NinoId == ninoId &&
                                a.Fecha.Month == mes &&
                                a.Fecha.Year == año &&
                                a.Asistio);

            return (decimal)asistenciasPositivas / totalAsistencias * 100;
        }
    }
}
