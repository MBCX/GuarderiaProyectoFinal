using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IIngredienteRepository
    {
        Task<List<Ingrediente>> GetAllAsync();
        Task<Ingrediente?> GetByNombreAsync(string nombre);
        Task<Ingrediente?> GetByIdAsync(int id);
        Task<List<Ingrediente>> GetAlergenosAsync();
        Task<List<Ingrediente>> GetNoAlergenosAsync();
        Task AddAsync(Ingrediente ingrediente);
        Task UpdateAsync(Ingrediente ingrediente);
        Task DeleteAsync(int id);
        Task<bool> ExisteAsync(string nombre);
        Task<bool> EsAlergenoAsync(int id);
    }
}
