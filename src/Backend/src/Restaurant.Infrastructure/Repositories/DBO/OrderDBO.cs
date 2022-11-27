namespace Restaurant.Infrastructure.Repositories.DBO
{
    internal sealed class OrderDBO
    {
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime Created { get; set; }
        public decimal Price { get; set; }
        public string? Email { get; set; }
        public string? Note { get; set; }
        public IList<ProductSaleDBO> ProductSales { get; set; } = new List<ProductSaleDBO>();
    }
}
