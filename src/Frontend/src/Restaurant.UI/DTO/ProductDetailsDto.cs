namespace Restaurant.UI.DTO
{
    public class ProductDetailsDto : ProductDto
    {
        public IEnumerable<OrderDto>? Orders { get; set; }
    }
}
