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
        Task<List<Nino>> ObtenerTodosLosNinosAsync();
        Task<List<Nino>> ObtenerNinosActivosAsync();
        Task<int> RegistrarNinoAsync(Nino nino);
        Task ActualizarNinoAsync(Nino nino);
        Task DarBajaNinoAsync(int NinoId, DateTime fechaBaja);
        Task<bool> ValidarDatosObligatoriosAsync(Nino nino);
    }
}
