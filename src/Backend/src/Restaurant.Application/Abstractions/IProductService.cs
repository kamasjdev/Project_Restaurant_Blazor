using Restaurant.Application.DTO;

namespace Restaurant.Application.Abstractions
{
    public interface IProductService
    {
        Task<ProductDetailsDto?> GetAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task AddAsync(ProductDto productDto);
        Task UpdateAsync(ProductDto productDto);
        Task DeleteAsync(Guid id);
    }
}
