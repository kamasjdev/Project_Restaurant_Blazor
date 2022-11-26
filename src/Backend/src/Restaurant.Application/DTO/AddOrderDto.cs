namespace Restaurant.Application.DTO
{
    public class AddOrderDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Note { get; set; }
        public IEnumerable<Guid>? ProductSaleIds { get; set; }
    }
}
