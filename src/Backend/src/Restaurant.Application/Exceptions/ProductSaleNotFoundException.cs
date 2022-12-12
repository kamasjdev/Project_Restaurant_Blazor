namespace Restaurant.Application.Exceptions
{
    public sealed class ProductSaleNotFoundException : BusinessException
    {
        public Guid ProductSaleId { get; }

        public ProductSaleNotFoundException(Guid productSaleId) : base($"ProductSale with id: '{productSaleId}' was not found")
        {

        }
    }
}
