namespace Restaurant.Application.Exceptions
{
    public sealed class CannotConstructEmailFromOrderException : BusinessException
    {
        public CannotConstructEmailFromOrderException() : base("Cannot construct email from null order")
        {
        }
    }
}
