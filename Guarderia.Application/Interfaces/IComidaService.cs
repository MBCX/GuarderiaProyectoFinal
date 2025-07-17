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
        Task<List<Comida>> ObtenerComidasPorNiñoAsync(int niñoId);
        Task<List<Comida>> ObtenerComidasPorFechaAsync(DateTime fecha);
        Task<List<Comida>> ObtenerComidasPorTipoAsync(string tipo);
        Task<List<Comida>> ObtenerComidasPorMesAsync(int niñoId, int mes, int año);
        Task AsignarComidaAsync(int niñoId, DateTime fecha, string tipoComida, decimal costo);
        Task ActualizarComidaAsync(Comida comida);
        Task<bool> ValidarAlergiaAntesDeComerAsync(int niñoId, string[] ingredientes);
        Task<bool> ValidarComidaContraAlergiasAsync(int niñoId, int menuId);
        Task<decimal> CalcularCostoComidasMensualAsync(int niñoId, int mes, int año);
        Task<int> ContarComidasMensualesAsync(int niñoId, int mes, int año);
        Task<List<Comida>> GenerarReporteConsumoDiarioAsync(DateTime fecha);
        Task<List<Comida>> GenerarReporteConsumoMensualAsync(int mes, int año);
        Task<bool> YaRegistradaAsync(int niñoId, DateTime fecha, string tipo);
        Task EliminarComidaAsync(int id);
        Task<List<string>> ObtenerTiposComidasAsync(); 
    }
}
