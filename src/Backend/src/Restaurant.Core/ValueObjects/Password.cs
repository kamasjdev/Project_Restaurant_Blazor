using Restaurant.Core.Exceptions;
using System.Text.RegularExpressions;

namespace Restaurant.Core.ValueObjects
{
    public class Password : IEquatable<Password>
    {
        public static readonly Regex PasswordRegex = new("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d\\w\\W]{8,}$");
        public string Value { get; }

        public Password(string password)
        {
            ValidPassword(password);
            Value = password;
        }

        public static implicit operator string(Password password)
            => password.Value;

        public static implicit operator Password(string value)
            => new(value);

        public override bool Equals(object? obj)
        {
            return Equals(obj as Password);
        }

        public bool Equals(Password? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                    .Select(x => x != null ? x.GetHashCode() : 0)
                    .Aggregate((x, y) => x ^ y);
        }

        private IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private static void ValidPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidPasswordException();
            }

            if (!PasswordRegex.IsMatch(password))
            {
                throw new InvalidPasswordException();
            }
        }
    }
}
