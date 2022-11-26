using Restaurant.Application.DTO;

namespace Restaurant.Application.Abstractions
{
    public interface IOrderService
    {
        Task<OrderDetailsDto?> GetAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task AddAsync(AddOrderDto addOrderDto);
        Task UpdateAsync(AddOrderDto addOrderDto);
        Task DeleteAsync(Guid id);
        Task DeleteWithPositionsAsync(Guid id);
    }
}
