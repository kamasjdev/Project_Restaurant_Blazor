using Restaurant.Core.Entities;

namespace Restaurant.Infrastructure.Repositories.DBO
{
    internal sealed class AdditionDBO
    {
        public Guid Id { get; set; }
        public string? AdditionName { get; set; }
        public decimal Price { get; set; }
        public AdditionKind AdditionKind { get; set; }
        public IList<ProductSaleDBO> ProductSales { get; set; } = new List<ProductSaleDBO>();
    }
}
