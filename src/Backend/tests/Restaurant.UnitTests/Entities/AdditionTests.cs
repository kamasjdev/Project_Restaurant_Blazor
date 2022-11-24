using Restaurant.Core.Entities;
using Restaurant.Core.Exceptions;
using Shouldly;

namespace Restaurant.UnitTests.Entities
{
    public class AdditionTests
    {
        [Fact]
        public void should_create_addition()
        {
            var id = Guid.NewGuid();
            var additionName = "addition#1";
            var price = 100;
            var additionKind = AdditionKind.Salad;

            var addition = new Addition(id, additionName, price, additionKind);

            addition.ShouldNotBeNull();
            addition.Id.Value.ShouldBe(id);
            addition.AdditionName.Value.ShouldBe(additionName);
            addition.Price.Value.ShouldBe(price);
            addition.AdditionKind.ShouldBe(additionKind);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("12")]
        public void given_invalid_addition_kind_should_throw_an_exception(string additionKind)
        {
            var id = Guid.NewGuid();
            var additionName = "addition#1";
            var price = 100;
            var expectedException = new InvalidAdditionKindException(additionKind);

            var exception = Record.Exception(() => new Addition(id, additionName, price, additionKind));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((InvalidAdditionKindException)exception).AdditionKind.ShouldBe(expectedException.AdditionKind);
        }
    }
}
