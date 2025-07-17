using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IAlergiaService
    {
        Task<List<string>> ObtenerAlergiasPorNiñoAsync(int niñoId);
        Task<List<Alergia>> ObtenerAlergiasCompletasPorNiñoAsync(int niñoId);
        Task RegistrarAlergiaAsync(int ninoId, string ingrediente);
        Task RegistrarAlergiaAsync(int ninoId, int ingredienteId);
        Task EliminarAlergiaAsync(int ninoId, string ingrediente);
        Task EliminarAlergiaAsync(int ninoId, int ingredienteId);
        Task ActualizarAlergiasAsync(int ninoId, List<string> nuevasAlergias);
        Task ActualizarAlergiasAsync(int ninoId, List<int> nuevosIngredienteIds);
        Task<bool> TieneAlergiaAIngredienteAsync(int ninoId, string ingrediente);
        Task<bool> TieneAlergiaAIngredienteAsync(int ninoId, int ingredienteId); // NUEVO
        Task<bool> PuedeConsumirPlatoAsync(int ninoId, int platoId); // NUEVO
        Task<bool> PuedeConsumirMenuAsync(int ninoId, int menuId); // NUEVO
        Task<List<string>> ValidarMenuContraAlergiasAsync(int ninoId, int menuId); // NUEVO
        Task<List<Nino>> ObtenerNiñosConAlergiasAsync(); // NUEVO
        Task<List<string>> ObtenerIngredientesAlergenosAsync(); // NUEVO
    }
}
