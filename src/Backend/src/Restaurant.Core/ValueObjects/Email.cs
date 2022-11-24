using Restaurant.Core.Exceptions;
using System.Text.RegularExpressions;

namespace Restaurant.Core.ValueObjects
{
    public sealed class Email : IEquatable<Email>
    {
        public const string EMAIL_PATTERN = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
        private readonly string _email;

        public string Value => _email;

        public Email(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new EmailCannotBeEmptyException();
            }

            if (!Regex.Match(email, EMAIL_PATTERN).Success)
            {
                throw new InvalidEmailException(email);
            }

            _email = email;
        }

        public static Email Of(string? email) => new Email(email);

        public bool Equals(Email? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _email == other._email;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Email)obj);
        }

        private IEnumerable<object> GetEqualityComponents()
        {
            yield return _email;
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                    .Select(x => x != null ? x.GetHashCode() : 0)
                    .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(Email? left, Email? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Email? left, Email? right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return _email;
        }
    }
}
