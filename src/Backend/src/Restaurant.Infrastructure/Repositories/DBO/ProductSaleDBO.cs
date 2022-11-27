using Restaurant.Core.Entities;

namespace Restaurant.Infrastructure.Repositories.DBO
{
    internal sealed class ProductSaleDBO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public ProductDBO? Product { get; set; }
        public Guid? AdditionId { get; set; }
        public AdditionDBO? Addition { get; set; }
        public decimal EndPrice { get; set; } = decimal.Zero;
        public Guid? OrderId { get; set; }
        public OrderDBO? Order { get; set; }
        public ProductSaleState ProductSaleState { get; set; } = ProductSaleState.New;
        public string? Email { get; set; }
    }
}
