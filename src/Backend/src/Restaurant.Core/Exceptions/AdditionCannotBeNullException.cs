namespace Restaurant.Core.Exceptions
{
    public sealed class AdditionCannotBeNullException : DomainException
    {
        public AdditionCannotBeNullException() : base("Additon cannot be null")
        {
        }
    }
}
