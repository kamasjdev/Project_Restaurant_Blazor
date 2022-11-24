using Restaurant.Core.Entities;

namespace Restaurant.Core.Repositories
{
    public interface IAdditonRepository
    {
        Task AddAsync(Addition addition);
        Task UpdateAsync(Addition addition);
        Task DeleteAsync(Addition addition);
        Task<Addition> GetAsync(Guid id);
        Task<IEnumerable<Addition>> GetAllAsync();
    }
}
