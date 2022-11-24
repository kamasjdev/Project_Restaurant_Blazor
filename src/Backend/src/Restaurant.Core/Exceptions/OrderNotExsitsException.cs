using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Exceptions
{
    public sealed class OrderNotExsitsException : DomainException
    {
        public EntityId ProductSaleId { get; }

        public OrderNotExsitsException(EntityId productSaleId) : base($"Order not exists in ProductSale: {productSaleId.Value}")
        {
            ProductSaleId = productSaleId;
        }
    }
}
