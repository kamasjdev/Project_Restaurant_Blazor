using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Entities
{
    public class User
    {
        public EntityId Id { get; }
        public Email Email { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }
        public DateTime CreatedAt { get; }

        public User(EntityId id, Email email, string password, string role, DateTime createdAt)
        {
            Id = id;
            ChangeEmail(email);
            ChangePassword(password);
            ChangeRole(role);
            CreatedAt = createdAt;
        }

        public void ChangeEmail(Email email)
        {
            Email = email;
        }

        public void ChangePassword(string password)
        {
            Password = password;
        }

        public void ChangeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new InvalidRoleException(role);
            }

            if (role != Roles.UserRole && role != Roles.AdminRole)
            {
                throw new InvalidRoleException(role);
            }

            Role = role;
        }

        public static class Roles
        {
            public const string UserRole = "user";
            public const string AdminRole = "admin";
        }

        public static User Create(Email email, string password, DateTime createdAt)
        {
            return new User(Guid.NewGuid(), email, password, Roles.UserRole, createdAt);
        }

        public static User Create(Email email, string password, string role, DateTime createdAt)
        {
            return new User(Guid.NewGuid(), email, password, role, createdAt);
        }
    }
}
