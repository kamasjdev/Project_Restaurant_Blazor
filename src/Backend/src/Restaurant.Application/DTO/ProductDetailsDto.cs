namespace Restaurant.Application.DTO
{
    public class ProductDetailsDto : ProductDto
    {
        public IEnumerable<OrderDto>? Orders { get; set; }
    }
}
