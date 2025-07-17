using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;


namespace Guarderia.Application.Interfaces
{
    public interface INinoService
    {
        Task<Nino> ObtenerNiñoPorIdAsync(int id);
        Task<Nino> ObtenerPorMatriculaAsync(string numeroMatricula);
        Task<List<Nino>> ObtenerTodosLosNiñosAsync();
        Task<List<Nino>> ObtenerNiñosActivosAsync();
        Task<List<Nino>> ObtenerNiñosInactivosAsync();
        Task<List<Nino>> ObtenerPorResponsablePagoAsync(int responsablePagoId);
        Task<int> RegistrarNiñoAsync(Nino nino, int responsablePagoId);
        Task ActualizarNiñoAsync(Nino nino);
        Task DarBajaNiñoAsync(int niñoId, DateTime fechaBaja);
        Task ReactivarNiñoAsync(int niñoId);
        Task AsignarResponsablePagoAsync(int niñoId, int responsablePagoId);
        Task<bool> ValidarDatosObligatoriosAsync(Nino nino);
        Task<bool> ExisteMatriculaAsync(string numeroMatricula);
        Task<string> GenerarNumeroMatriculaAsync();
        Task<int> ContarNiñosActivosAsync();
        Task<List<string>> ObtenerAlergiasAsync(int niñoId);
        Task EliminarNiñoAsync(int id);
    }
}
