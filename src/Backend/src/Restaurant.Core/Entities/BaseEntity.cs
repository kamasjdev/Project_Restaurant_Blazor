using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Entities
{
    public class BaseEntity
    {
        public EntityId Id { get; protected set; }

        public BaseEntity(EntityId? id)
        {
            if (id is null)
            {
                throw new InvalidEntityIdException(Guid.Empty);
            }

            Id = id;
        }
    }
}
