namespace Restaurant.UI.DTO
{
    public class AddProductSaleDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid? AdditionId { get; set; }
        public Guid? OrderId { get; set; }
        public string? Email { get; set; }
    }
}
