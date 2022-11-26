namespace Restaurant.Application.Exceptions
{
    public sealed class CannotUpdateProductSaleException : ApplicationException
    {
        public Guid ProductSaleId { get; }
        public string ProductSaleState { get; }

        public CannotUpdateProductSaleException(Guid productSaleId, string productSaleState) : base($"Cannot update ProductSale with id: '{productSaleId}' and state: {productSaleState}")
        {
            ProductSaleId = productSaleId;
            ProductSaleState = productSaleState;
        }
    }
}
