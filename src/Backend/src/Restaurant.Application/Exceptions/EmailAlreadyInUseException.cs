namespace Restaurant.Application.Exceptions
{
    public sealed class EmailAlreadyInUseException : BusinessException
    {
        public string Email { get; }

        public EmailAlreadyInUseException(string email) : base($"Email: '{email}' is already in use")
        {
            Email = email;
        }
    }
}
