using Restarant.Application.DTO;

namespace Restarant.Application.Abstractions
{
    public interface IAdditionService
    {
        Task AddAsync(AdditionDto additionDto);
        Task UpdateAsync(AdditionDto additionDto);
        Task DeleteAsync(Guid id);
        Task<AdditionDto?> GetAsync(Guid id);
        Task<IEnumerable<AdditionDto>> GetAllAsync();
    }
}
