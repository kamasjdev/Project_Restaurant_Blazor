using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void should_create_email()
        {
            var emailName = "test@test.com";

            var email = new Email(emailName);

            email.Value.ShouldBe(emailName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void given_null_empty_or_with_white_space_email_should_throw_an_exception(string emailName)
        {
            var expectedException = new EmailCannotBeEmptyException();

            var exception = Record.Exception(() => new Email(emailName));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Theory]
        [InlineData("email")]
        [InlineData("email@")]
        public void given_invalid_email_should_throw_an_exception(string emailName)
        {
            var expectedException = new InvalidEmailException(emailName);

            var exception = Record.Exception(() => new Email(emailName));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((InvalidEmailException)exception).Email.ShouldBe(expectedException.Email);
        }
    }
}
