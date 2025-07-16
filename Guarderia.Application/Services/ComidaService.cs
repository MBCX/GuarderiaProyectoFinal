using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Application.Interfaces;
using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;

namespace Guarderia.Application.Services
{
    public class ComidaService : IComidaService
    {
        private readonly IComidaRepository _comidaRepository;
        private readonly IAlergiaService _alergiaService;

        public ComidaService(IComidaRepository comidaRepository, IAlergiaService alergiaService)
        {
            _comidaRepository = comidaRepository;
            _alergiaService = alergiaService;
        }

        public async Task<List<Comida>> ObtenerComidasPorNinoAsync(int NinoId)
        {
            return await _comidaRepository.ObtenerPorNinoIdAsync(NinoId);
        }

        public async Task<List<Comida>> ObtenerComidasPorFechaAsync(DateTime fecha)
        {
            var todasComidas = await _comidaRepository.ObtenerTodasAsync();
            return todasComidas.Where(c => c.Fecha.Date == fecha.Date).ToList();
        }

        public async Task AsignarComidaAsync(int NinoId, DateTime fecha, string tipoComida, decimal costo)
        {
            var comida = new Comida
            {
                NinoId = NinoId,
                Fecha = fecha,
                Tipo = tipoComida,
                Costo = costo
            };

            await _comidaRepository.RegistrarAsync(comida);
        }

        public async Task<bool> ValidarAlergiaAntesDeComerAsync(int NinoId, string[] ingredientes)
        {
            var alergias = await _alergiaService.ObtenerAlergiasPorNinoAsync(NinoId);

            foreach (var ingrediente in ingredientes)
            {
                if (alergias.Contains(ingrediente, StringComparer.OrdinalIgnoreCase))
                {
                    // Tiene alergia a al menos un ingrediente
                    return false;
                }
            }

            // No tiene alergias a ningun ingrediente
            return true;
        }

        public async Task<decimal> CalcularCostoComidasMensualAsync(int NinoId, int mes, int año)
        {
            var comidas = await _comidaRepository.ObtenerPorNinoIdAsync(NinoId);

            return comidas
                .Where(c => c.Fecha.Month == mes && c.Fecha.Year == año)
                .Sum(c => c.Costo);
        }

        public async Task<List<Comida>> GenerarReporteConsumoDiarioAsync(DateTime fecha)
        {
            return await ObtenerComidasPorFechaAsync(fecha);
        }
    }
}
