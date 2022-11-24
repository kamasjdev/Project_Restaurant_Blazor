namespace Restaurant.Core.Exceptions
{
    public sealed class ProductNameTooShortException : DomainException
    {
        public string ProductName { get; }

        public ProductNameTooShortException(string productName) : base($"ProductName: '{productName}' should have at least 3 characters")
        {
            ProductName = productName;
        }
    }
}
