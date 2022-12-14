using Restaurant.UI.DTO;

namespace Restaurant.UI.Services.Abstractions
{
	public interface IAdditionService
	{
		Task<Guid> AddAsync(AdditionDto additionDto);
		Task UpdateAsync(AdditionDto additionDto);
		Task DeleteAsync(Guid id);
		Task<AdditionDto?> GetAsync(Guid id);
		Task<IEnumerable<AdditionDto>> GetAllAsync();
	}
}
