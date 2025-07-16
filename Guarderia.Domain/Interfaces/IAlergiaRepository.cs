using Guarderia.Domain.Entities;

namespace Guarderia.Domain.Interfaces
{
    public interface IAlergiaRepository
    {
        Task<List<Alergia>> GetAllAsync();
        Task<Alergia?> GetByIdAsync(int id);
        Task<List<Alergia>> GetByNinoIdAsync(int NinoId);
        Task<List<string>> GetIngredientesAlergenosPorNinoAsync(int NinoId);
        Task<bool> TieneAlergiaAIngredienteAsync(int NinoId, int ingredienteId);
        Task<bool> TieneAlergiaAIngredienteAsync(int NinoId, string nombreIngrediente);
        Task AddAsync(Alergia alergia);
        Task DeleteAsync(int id);
        Task DeleteByNinoEIngredienteAsync(int NinoId, int ingredienteId);
        Task<List<Nino>> GetNinosConAlergiasAsync();
    }
}