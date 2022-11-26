namespace Restaurant.Application.Exceptions
{
    public sealed class UserNotFoundException : ApplicationException
    {
        public Guid UserId { get; }

        public UserNotFoundException(Guid userId) : base($"User with id: '{userId}' was not found")
        {
            UserId = userId;
        }
    }
}
