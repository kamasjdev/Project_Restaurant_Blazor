using Grpc.Core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ExceptionExtensions;
using Restaurant.UI.DTO;
using Restaurant.UI.Pages;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.UnitTests.Pages
{
    public class SignInPageTests
    {
        [Fact]
        public void should_render_sign_in_page()
        {
            _component.Markup.Contains("Login");
        }

        [Fact]
        public void given_invalid_email_should_show_error()
        {
            var emailInput = _component.Find("""input[type="email"]""");
            var instance = _component.Instance;

            emailInput.Change("test");

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationEmailMessages = instance.GetValidationMessage(instance.EditContext, () => instance.Form.Email);
            validationEmailMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationEmailMessages);
        }

        [Fact]
        public void given_empty_password_should_show_error()
        {
            var password = _component.Find("""input[type="password"]""");
            var instance = _component.Instance;

            password.Change("a");
            password.Change("");

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationPasswordMessages = instance.GetValidationMessage(instance.EditContext, () => instance.Form.Password);
            validationPasswordMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationPasswordMessages);
        }

        [Fact]
        public void given_invalid_all_fields_should_show_errors()
        {
            var submitButton = _component.Find("""button[type="submit"]""");
            var instance = _component.Instance;

            submitButton.Click();

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationEmailMessages = instance.GetValidationMessage(instance.EditContext, () => instance.Form.Email);
            var validationPasswordMessages = instance.GetValidationMessage(instance.EditContext, () => instance.Form.Password);
            validationEmailMessages.ShouldNotBeNullOrWhiteSpace();
            validationPasswordMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationEmailMessages);
            _component.Markup.ShouldContain(validationPasswordMessages);
        }

        [Fact]
        public async Task should_sign_in()
        {
            var submitButton = _component.Find("""button[type="submit"]""");
            var instance = _component.Instance;
            var emailInput = _component.Find("""input[type="email"]""");
            var password = _component.Find("""input[type="password"]""");
            emailInput.Change("email@email.com");
            password.Change("password");

            await submitButton.ClickAsync(new MouseEventArgs());

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var errors = instance.EditContext.GetValidationMessages();
            errors.ShouldBeEmpty();
            instance.Error.ShouldBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_credentials_should_show_error()
        {
            var submitButton = _component.Find("""button[type="submit"]""");
            var instance = _component.Instance;
            var emailInput = _component.Find("""input[type="email"]""");
            var password = _component.Find("""input[type="password"]""");
            emailInput.Change("email@email.com");
            password.Change("password");
            var expectedException = new RpcException(new Status(StatusCode.FailedPrecondition, "Invalid Credentials"));
            _authenticationService.SignInAsync(Arg.Any<SignInDto>())
                .ThrowsAsyncForAnyArgs(expectedException);

            await submitButton.ClickAsync(new MouseEventArgs());

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var errors = instance.EditContext.GetValidationMessages();
            errors.ShouldBeEmpty();
            instance.Error.ShouldNotBeNullOrWhiteSpace();
            instance.Error.ShouldContain(expectedException.Status.Detail);
        }

        private readonly TestContext _testContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IRenderedComponent<SignInPage> _component;

        public SignInPageTests()
        {
            _testContext = new TestContext();
            _testContext.AddTestAuthorization();
            _authenticationService = Substitute.For<IAuthenticationService>();
            _testContext.Services.AddScoped(_ => _authenticationService);
            _component = _testContext.RenderComponent<SignInPage>();
        }
    }
}
