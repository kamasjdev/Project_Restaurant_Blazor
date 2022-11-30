namespace Restaurant.UI.DTO
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public string? ProductKind { get; set; }
    }
}
