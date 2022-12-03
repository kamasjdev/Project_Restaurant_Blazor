using Restaurant.UI.DTO;

namespace Restaurant.UI.Services.Abstractions
{
    public interface IProductService
    {
        Task<Guid> AddAsync(ProductDto productDto);
        Task UpdateAsync(ProductDto productDto);
        Task DeleteAsync(Guid id);
        Task<ProductDto?> GetAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
    }
}
