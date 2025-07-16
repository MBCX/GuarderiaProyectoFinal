using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Application.Interfaces
{
    public interface IAlergiaService
    {
        Task<List<string>> ObtenerAlergiasPorNinoAsync(int NinoId);
        Task RegistrarAlergiaAsync(int ninoId, string ingrediente);
        Task EliminarAlergiaAsync(int ninoId, string ingrediente);
        Task ActualizarAlergiasAsync(int ninoId, List<string> nuevasAlergias);
        Task<bool> TieneAlergiaAIngredienteAsync(int ninoId, string ingrediente);
    }
}
