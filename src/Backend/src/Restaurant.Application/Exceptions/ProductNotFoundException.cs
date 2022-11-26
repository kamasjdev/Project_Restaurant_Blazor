namespace Restaurant.Application.Exceptions
{
    public sealed class ProductNotFoundException : ApplicationException
    {
        public Guid ProductId { get; }

        public ProductNotFoundException(Guid productId) : base($"Product with id: '{productId}' was not found")
        {
            ProductId = productId;
        }
    }
}
