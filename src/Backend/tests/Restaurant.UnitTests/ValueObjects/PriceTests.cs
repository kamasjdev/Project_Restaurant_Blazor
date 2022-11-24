using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.ValueObjects
{
    public class PriceTests
    {
        [Fact]
        public void should_create_price()
        {
            var cost = 100M;

            var price = new Price(cost);

            price.ShouldNotBeNull();
            price.Value.ShouldBe(cost);
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(-10)]
        public void given_negative_cost_should_throw_an_exception(decimal cost)
        {
            var expectedException = new PriceCannotBeNegativeException(cost);

            var exception = Record.Exception(() => new Price(cost));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((PriceCannotBeNegativeException)exception).Price.ShouldBe(cost);
        }
    }
}
