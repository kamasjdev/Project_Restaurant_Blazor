using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.ValueObjects
{
    public class ProductNameTests
    {
        [Fact]
        public void should_create_product_name()
        {
            var name = "Name#1";

            var productName = new ProductName(name);

            productName.ShouldNotBeNull();
            productName.Value.ShouldBe(name);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("a2")]
        public void given_too_short_product_name_should_throw_an_exception(string name)
        {
            var expectedException = new ProductNameTooShortException(name);

            var exception = Record.Exception(() => new ProductName(name));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((ProductNameTooShortException)exception).ProductName.ShouldBe(expectedException.ProductName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void given_invalid_product_name_should_throw_an_exception(string name)
        {
            var expectedException = new ProductNameCannotBeEmptyException();

            var exception = Record.Exception(() => new ProductName(name));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }
    }

}
