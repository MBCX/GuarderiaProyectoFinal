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
        Task<List<Nino>> ObtenerTodosLosNiñosAsync();
        Task<List<Nino>> ObtenerNiñosActivosAsync();
        Task<int> RegistrarNiñoAsync(Nino nino);
        Task ActualizarNiñoAsync(Nino nino);
        Task DarBajaNiñoAsync(int niñoId, DateTime fechaBaja);
        Task<bool> ValidarDatosObligatoriosAsync(Nino nino);
    }
}
