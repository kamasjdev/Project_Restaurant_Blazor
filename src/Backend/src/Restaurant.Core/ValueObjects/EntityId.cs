using Restaurant.Core.Exceptions;

namespace Restaurant.Core.ValueObjects
{
    public class EntityId : IEquatable<EntityId>
    {
        public Guid Value { get; }

        public EntityId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new InvalidEntityIdException(value);
            }

            Value = value;
        }

        public static EntityId Create() => new(Guid.NewGuid());

        public override bool Equals(object? obj)
        {
            return Equals(obj as EntityId);
        }

        public bool Equals(EntityId? other)
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

        public static implicit operator Guid(EntityId date)
            => date.Value;

        public static implicit operator EntityId(Guid value)
            => new(value);
    }
}