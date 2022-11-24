using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.ValueObjects
{
    public class AdditionNameTests
    {
        [Fact]
        public void should_create_addition_name()
        {
            var name = "Name#1";

            var additionName = new AdditionName(name);

            additionName.ShouldNotBeNull();
            additionName.Value.ShouldBe(name);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("a2")]
        public void given_too_short_addition_name_should_throw_an_exception(string name)
        {
            var expectedException = new AdditionNameTooShortException(name);

            var exception = Record.Exception(() => new AdditionName(name));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((AdditionNameTooShortException)exception).AdditionName.ShouldBe(expectedException.AdditionName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void given_invalid_addition_name_should_throw_an_exception(string name)
        {
            var expectedException = new AdditionNameCannotBeEmptyException();
            var exception = Record.Exception(() => new AdditionName(name));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }
    }
}
