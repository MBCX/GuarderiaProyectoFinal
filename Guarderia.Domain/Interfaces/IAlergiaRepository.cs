using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IAlergiaRepository
    {
        Task<List<Alergia>> GetAllAsync();
        Task<Alergia?> GetByIdAsync(int id);
        Task<List<Alergia>> GetByNiñoIdAsync(int niñoId);
        Task AddAsync(Alergia alergia);
        Task DeleteAsync(int id);
    }
}