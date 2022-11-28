namespace Restaurant.Core.Exceptions
{
    public sealed class InvalidRoleException : DomainException
    {
        public string? Role { get; }

        public InvalidRoleException(string? role) : base($"Invalid role: '{role}'")
        {
            Role = role;
        }
    }
}
