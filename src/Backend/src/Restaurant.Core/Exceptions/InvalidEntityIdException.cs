namespace Restaurant.Core.Exceptions
{
    public sealed class InvalidEntityIdException : DomainException
    {
        public Guid Id { get; }

        public InvalidEntityIdException(Guid id) : base($"Cannot set: '{id}' as entity identifier.")
        {
        }
    }
}
