using Restaurant.UI.DTO;

namespace Restaurant.UI.Services.Abstractions
{
    public interface IProductSaleService
    {
        Task<Guid> AddAsync(ProductSaleDto productSaleDto);
        Task DeleteAsync(Guid id);
    }
}
