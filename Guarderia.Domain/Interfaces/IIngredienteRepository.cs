using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IIngredienteRepository
    {
        Task<List<Ingrediente>> GetAllAsync();
        Task<Ingrediente?> GetByNombreAsync(string nombre);
        Task<Ingrediente?> GetByIdAsync(int id);
        Task AddAsync(Ingrediente ingrediente);
        Task UpdateAsync(Ingrediente ingrediente);
        Task DeleteAsync(int id);
    }
}
