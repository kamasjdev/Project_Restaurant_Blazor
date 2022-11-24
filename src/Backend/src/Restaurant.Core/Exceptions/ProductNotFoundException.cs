using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Exceptions
{
    public sealed class ProductNotFoundException : DomainException
    {
        public EntityId ProductId { get; }

        public ProductNotFoundException(EntityId productId) : base($"Product with id: '{productId}' was not found")
        {
            ProductId = productId;
        }
    }
}
