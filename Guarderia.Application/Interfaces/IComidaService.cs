using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IComidaService
    {
        Task<List<Comida>> ObtenerComidasPorNinoAsync(int NinoId);
        Task<List<Comida>> ObtenerComidasPorFechaAsync(DateTime fecha);
        Task AsignarComidaAsync(int ninoId, DateTime fecha, string tipoComida, decimal costo);
        Task<bool> ValidarAlergiaAntesDeComerAsync(int ninoId, string[] ingredientes);
        Task<decimal> CalcularCostoComidasMensualAsync(int ninoId, int mes, int año);
        Task<List<Comida>> GenerarReporteConsumoDiarioAsync(DateTime fecha);
    }
}
