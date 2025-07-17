using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IIngredienteService
    {
        Task<Ingrediente> ObtenerPorIdAsync(int id);
        Task<Ingrediente> ObtenerPorNombreAsync(string nombre);
        Task<List<Ingrediente>> ObtenerTodosAsync();
        Task<List<Ingrediente>> ObtenerAlergenosAsync();
        Task<List<Ingrediente>> ObtenerNoAlergenosAsync();
        Task<int> CrearIngredienteAsync(Ingrediente ingrediente);
        Task ActualizarIngredienteAsync(Ingrediente ingrediente);
        Task MarcarComoAlergenoAsync(int id);
        Task DesmarcarComoAlergenoAsync(int id);
        Task<bool> ExisteAsync(string nombre);
        Task<bool> EsAlergenoAsync(int id);
        Task<bool> ValidarNombreAsync(string nombre);
        Task EliminarIngredienteAsync(int id);
        Task<List<Plato>> ObtenerPlatosQueContienenAsync(int ingredienteId);
    }
}
