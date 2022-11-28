using Restaurant.Core.Entities;

namespace Restaurant.Infrastructure.Repositories.DBO
{
    internal sealed class ProductDBO
    {
        public Guid Id { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public ProductKind ProductKind { get; set; }
        public IList<ProductSaleDBO> ProductSales { get; set; } = new List<ProductSaleDBO>();
    }
}
