namespace Restaurant.UI.DTO
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime Created { get; set; }
        public decimal Price { get; set; }
        public string? Email { get; set; }
        public string? Note { get; set; }
    }
}
