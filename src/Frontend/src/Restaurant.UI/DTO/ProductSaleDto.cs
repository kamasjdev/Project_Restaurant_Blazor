namespace Restaurant.UI.DTO
{
    public class ProductSaleDto
    {
        public Guid Id { get; set; }
        public AdditionDto? Addition { get; set; }
        public ProductDto? Product { get; set; }
        public decimal EndPrice { get; set; }
        public OrderDto? Order { get; set; }
        public string? ProductSaleState { get; set; }
        public string? Email { get; set; }
    }
}
