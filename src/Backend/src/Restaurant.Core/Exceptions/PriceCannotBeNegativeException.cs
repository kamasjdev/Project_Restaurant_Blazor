namespace Restaurant.Core.Exceptions
{
    public sealed class PriceCannotBeNegativeException : DomainException
    {
        public decimal Price { get; }

        public PriceCannotBeNegativeException(decimal price) : base($"Price: '{price}' cannot be negative")
        {
            Price = price;
        }
    }
}
