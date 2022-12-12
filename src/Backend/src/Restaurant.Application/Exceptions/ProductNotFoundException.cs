namespace Restaurant.Application.Exceptions
{
    public sealed class ProductNotFoundException : BusinessException
    {
        public Guid ProductId { get; }

        public ProductNotFoundException(Guid productId) : base($"Product with id: '{productId}' was not found")
        {
            ProductId = productId;
        }
    }
}
