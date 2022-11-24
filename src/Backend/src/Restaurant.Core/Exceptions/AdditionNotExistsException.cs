using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Exceptions
{
    public sealed class AdditionNotExistsException : DomainException
    {
        public EntityId ProductSaleId { get; }

        public AdditionNotExistsException(EntityId productSaleId) : base($"Addition not exists in ProductSale: '{productSaleId.Value}'")
        {
            ProductSaleId = productSaleId;
        }
    }
}
