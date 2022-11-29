using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Entities
{
    public class Addition : BaseEntity
    {
        public AdditionName AdditionName { get; private set; }
        public Price Price { get; private set; }
        public AdditionKind AdditionKind { get; private set; }

        private IList<EntityId> _productSaleIds = new List<EntityId>();
        public IEnumerable<EntityId> ProductSaleIds => _productSaleIds;

        public Addition(EntityId? id, AdditionName? additionName, Price? price, AdditionKind additionKind, IEnumerable<EntityId>? productSaleIds = null)
            : base(id)
        {
            ChangeAdditionName(additionName);
            ChangePrice(price);
            AdditionKind = additionKind;
            _productSaleIds = productSaleIds?.ToList() ?? new List<EntityId>();
        }

        public Addition(EntityId? id, AdditionName? additionName, Price? price, string additionKind, IEnumerable<EntityId>? productSaleIds = null)
            : base(id)
        {
            ChangeAdditionName(additionName);
            ChangePrice(price);
            ChangeAdditionKind(additionKind);
            _productSaleIds = productSaleIds?.ToList() ?? new List<EntityId>();
        }

        public void ChangeAdditionName(AdditionName? additionName)
        {
            if (additionName is null)
            {
                throw new AdditionNameCannotBeNullException();
            }

            AdditionName = additionName;
        }

        public void ChangePrice(Price? price)
        {
            if (price is null)
            {
                throw new PriceCannotBeNullException();
            }

            Price = price;
        }

        public void ChangeAdditionKind(string? additionKind)
        {
            var parsed = Enum.TryParse<AdditionKind>(additionKind, out var additionKindParsed);

            if (!parsed)
            {
                throw new InvalidAdditionKindException(additionKind);
            }

            if (!Enum.IsDefined(additionKindParsed))
            {
                throw new InvalidAdditionKindException(additionKind);
            }

            AdditionKind = additionKindParsed;
        }
    }
}
