using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Exceptions
{
    public sealed class CannotOverrideExistingOrderException : DomainException
    {
        public EntityId ProductSaleId { get; }

        public CannotOverrideExistingOrderException(EntityId productSaleId) : base($"Cannot override existing Order in ProductSale: '{productSaleId}'")
        {
            ProductSaleId = productSaleId;
        }
    }
}
