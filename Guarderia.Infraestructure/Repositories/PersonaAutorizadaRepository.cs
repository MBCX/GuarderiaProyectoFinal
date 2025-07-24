using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class PersonaAutorizadaRepository : IPersonaAutorizadaRepository
    {
        private readonly AppDbContext _context;

        public PersonaAutorizadaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PersonaAutorizada> ObtenerPorIdAsync(int id)
        {
            return await _context.PersonaAutorizadas
                .Include(p => p.NinosAutorizados)
                    .ThenInclude(na => na.Nino)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PersonaAutorizada> ObtenerPorCedulaAsync(string cedula)
        {
            return await _context.PersonaAutorizadas
                .Include(p => p.NinosAutorizados)
                    .ThenInclude(na => na.Nino)
                .FirstOrDefaultAsync(p => p.Cedula == cedula);
        }

        public async Task<List<PersonaAutorizada>> ObtenerTodasAsync()
        {
            return await _context.PersonaAutorizadas
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<List<PersonaAutorizada>> ObtenerPorNinoIdAsync(int ninoId)
        {
            return await _context.PersonaAutorizadas
                .Where(p => p.NinosAutorizados.Any(na => na.NinoId == ninoId))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<List<PersonaAutorizada>> ObtenerActivasPorNinoIdAsync(int ninoId)
        {
            return await _context.PersonaAutorizadas
                .Where(p => p.NinosAutorizados.Any(na => na.NinoId == ninoId && na.Activa))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task AgregarAsync(PersonaAutorizada personaAutorizada)
        {
            await _context.PersonaAutorizadas.AddAsync(personaAutorizada);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(PersonaAutorizada personaAutorizada)
        {
            _context.PersonaAutorizadas.Update(personaAutorizada);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var persona = await _context.PersonaAutorizadas.FindAsync(id);
            if (persona != null)
            {
                _context.PersonaAutorizadas.Remove(persona);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistePorCedulaAsync(string cedula)
        {
            return await _context.PersonaAutorizadas
                .AnyAsync(p => p.Cedula == cedula);
        }

        public async Task<bool> EstaAutorizadaParaNinoAsync(string cedula, int ninoId)
        {
            return await _context.PersonaAutorizadas
                .AnyAsync(p => p.Cedula == cedula &&
                              p.NinosAutorizados.Any(na => na.NinoId == ninoId && na.Activa));
        }
    }
}
