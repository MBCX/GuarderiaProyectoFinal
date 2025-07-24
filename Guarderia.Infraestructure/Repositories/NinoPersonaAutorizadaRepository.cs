using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Repositories
{
    public class NinoPersonaAutorizadaRepository : INinoPersonaAutorizadaRepository
    {
        private readonly AppDbContext _context;

        public NinoPersonaAutorizadaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NinoPersonaAutorizada>> ObtenerPorNinoIdAsync(int ninoId)
        {
            return await _context.NinoPersonaAutorizadas
                .Include(npa => npa.PersonaAutorizada)
                .Where(npa => npa.NinoId == ninoId)
                .OrderByDescending(npa => npa.FechaAutorizacion)
                .ToListAsync();
        }

        public async Task<List<NinoPersonaAutorizada>> ObtenerPorPersonaIdAsync(int personaAutorizadaId)
        {
            return await _context.NinoPersonaAutorizadas
                .Include(npa => npa.Nino)
                .Where(npa => npa.PersonaAutorizadaId == personaAutorizadaId)
                .OrderByDescending(npa => npa.FechaAutorizacion)
                .ToListAsync();
        }

        public async Task<List<NinoPersonaAutorizada>> ObtenerActivasPorNinoAsync(int ninoId)
        {
            return await _context.NinoPersonaAutorizadas
                .Include(npa => npa.PersonaAutorizada)
                .Where(npa => npa.NinoId == ninoId && npa.Activa)
                .OrderBy(npa => npa.PersonaAutorizada.Nombre)
                .ToListAsync();
        }

        public async Task<NinoPersonaAutorizada> ObtenerRelacionAsync(int ninoId, int personaAutorizadaId)
        {
            return await _context.NinoPersonaAutorizadas
                .Include(npa => npa.Nino)
                .Include(npa => npa.PersonaAutorizada)
                .FirstOrDefaultAsync(npa => npa.NinoId == ninoId && npa.PersonaAutorizadaId == personaAutorizadaId);
        }

        public async Task AgregarAsync(NinoPersonaAutorizada relacion)
        {
            await _context.NinoPersonaAutorizadas.AddAsync(relacion);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(NinoPersonaAutorizada relacion)
        {
            _context.NinoPersonaAutorizadas.Update(relacion);
            await _context.SaveChangesAsync();
        }

        public async Task RevocarAutorizacionAsync(int ninoId, int personaAutorizadaId, DateTime fechaRevocacion)
        {
            var relacion = await _context.NinoPersonaAutorizadas
                .FirstOrDefaultAsync(npa => npa.NinoId == ninoId && npa.PersonaAutorizadaId == personaAutorizadaId);

            if (relacion != null)
            {
                relacion.FechaRevocacion = fechaRevocacion;
                relacion.Activa = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EstaAutorizadaAsync(int ninoId, int personaAutorizadaId)
        {
            return await _context.NinoPersonaAutorizadas
                .AnyAsync(npa => npa.NinoId == ninoId &&
                                npa.PersonaAutorizadaId == personaAutorizadaId &&
                                npa.Activa);
        }

        public async Task<bool> PuedeRecogerNinoAsync(string cedulaPersona, int ninoId)
        {
            return await _context.NinoPersonaAutorizadas
                .Include(npa => npa.PersonaAutorizada)
                .AnyAsync(npa => npa.NinoId == ninoId &&
                                npa.PersonaAutorizada.Cedula == cedulaPersona &&
                                npa.Activa);
        }
    }
}
