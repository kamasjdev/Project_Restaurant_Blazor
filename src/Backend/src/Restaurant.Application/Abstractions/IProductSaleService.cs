using Restaurant.Application.DTO;

namespace Restaurant.Application.Abstractions
{
    public interface IProductSaleService
    {
        Task AddAsync(AddProductSaleDto productSaleDto);
        Task UpdateAsync(AddProductSaleDto productSaleDto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ProductSaleDto>> GetAllByOrderIdAsync(Guid orderId);
        Task<ProductSaleDto?> GetAsync(Guid productSaleId);
        Task<IEnumerable<ProductSaleDto>> GetAllByEmailAsync(string email);
    }
}
