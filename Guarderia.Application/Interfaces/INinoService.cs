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
        Task<Nino> ObtenerNinoPorIdAsync(int id);
        Task<Nino> ObtenerPorMatriculaAsync(string numeroMatricula);
        Task<List<Nino>> ObtenerTodosLosNinosAsync();
        Task<List<Nino>> ObtenerNinosActivosAsync();
        Task<List<Nino>> ObtenerNinosInactivosAsync();
        Task<List<Nino>> ObtenerPorResponsablePagoAsync(int responsablePagoId);
        Task<int> RegistrarNinoAsync(Nino nino, int responsablePagoId);
        Task ActualizarNinoAsync(Nino nino);
        Task DarBajaNinoAsync(int ninoId, DateTime fechaBaja);
        Task ReactivarNinoAsync(int ninoId);
        Task AsignarResponsablePagoAsync(int ninoId, int responsablePagoId);
        Task<bool> ValidarDatosObligatoriosAsync(Nino nino);
        Task<bool> ExisteMatriculaAsync(string numeroMatricula);
        Task<string> GenerarNumeroMatriculaAsync();
        Task<int> ContarNinosActivosAsync();
        Task<List<string>> ObtenerAlergiasAsync(int ninoId);
        Task EliminarNinoAsync(int id);
    }
}
