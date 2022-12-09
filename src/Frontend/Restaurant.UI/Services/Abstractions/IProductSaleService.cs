using Restaurant.UI.DTO;

namespace Restaurant.UI.Services.Abstractions
{
    public interface IProductSaleService
    {
        Task<Guid> AddAsync(ProductSaleDto productSaleDto);
        Task DeleteAsync(Guid id);
        Task<ProductSaleDto?> GetAsync(Guid productSaleId);
        Task<IEnumerable<ProductSaleDto>> GetAllInCartByEmailAsync(string email);
    }
}
