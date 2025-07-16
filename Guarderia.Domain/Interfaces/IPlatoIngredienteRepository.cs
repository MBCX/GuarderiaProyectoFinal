using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IPlatoIngredienteRepository
    {
        Task<List<PlatoIngrediente>> ObtenerPorPlatoIdAsync(int platoId);
        Task<List<PlatoIngrediente>> ObtenerPorIngredienteIdAsync(int ingredienteId);
        Task<PlatoIngrediente> ObtenerRelacionAsync(int platoId, int ingredienteId);
        Task AgregarAsync(PlatoIngrediente platoIngrediente);
        Task ActualizarAsync(PlatoIngrediente platoIngrediente);
        Task EliminarAsync(int platoId, int ingredienteId);
        Task EliminarPorPlatoAsync(int platoId);
        Task<List<Ingrediente>> ObtenerIngredientesDePlatoAsync(int platoId);
        Task<List<string>> ObtenerNombresIngredientesDePlatoAsync(int platoId);
        Task<bool> PlatoContieneIngredienteAsync(int platoId, int ingredienteId);
    }
}
