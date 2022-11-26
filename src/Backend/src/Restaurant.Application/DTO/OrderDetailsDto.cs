namespace Restaurant.Application.DTO
{
    public class OrderDetailsDto : OrderDto
    {
        public IEnumerable<ProductSaleDto> Products { get; set; } = new List<ProductSaleDto>();
    }
}
