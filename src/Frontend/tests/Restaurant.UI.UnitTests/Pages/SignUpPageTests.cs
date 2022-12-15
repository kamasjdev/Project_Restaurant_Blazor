using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.UI.DTO;
using Restaurant.UI.Pages;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.UnitTests.Pages
{
    public class SignUpPageTests
    {
        [Fact]
        public void should_render_sign_up_page()
        {
            _component.Markup.ShouldContain("Register");
        }

        [Fact]
        public void given_empty_email_should_show_error()
        {
            var instance = _component.Instance;
            var emailInput = _component.Find("""input[type="email"]""");
            emailInput.Change("a");

            emailInput.Change("");

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationPasswordMessages = instance.EditContext.GetValidationMessage(() => instance.Form.Email);
            validationPasswordMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationPasswordMessages);
        }

        [Fact]
        public void given_invalid_email_should_show_error()
        {
            var instance = _component.Instance;
            var emailInput = _component.Find("""input[type="email"]""");

            emailInput.Change("abcdefgh");

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationPasswordMessages = instance.EditContext.GetValidationMessage(() => instance.Form.Email);
            validationPasswordMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationPasswordMessages);
        }

        [Fact]
        public void given_empty_password_should_show_error()
        {
            var instance = _component.Instance;
            var passwordInput = _component.Find("#password");
            passwordInput.Change("abcdefgh");

            passwordInput.Change("");

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationPasswordMessages = instance.EditContext.GetValidationMessage(() => instance.Form.Password);
            validationPasswordMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationPasswordMessages);
        }

        [Fact]
        public void given_invalid_password_should_show_error()
        {
            var instance = _component.Instance;
            var passwordInput = _component.Find("#password");

            passwordInput.Change("abcdefgh");

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationPasswordMessages = instance.EditContext.GetValidationMessage(() => instance.Form.Password);
            validationPasswordMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationPasswordMessages);
        }

        [Fact]
        public void given_empty_confirm_password_should_show_error()
        {
            var instance = _component.Instance;
            var confirmPasswordInput = _component.Find("#confirm-password");
            confirmPasswordInput.Change("abcdefgh");

            confirmPasswordInput.Change("");

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationPasswordMessages = instance.EditContext.GetValidationMessage(() => instance.Form.ConfirmPassword);
            validationPasswordMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationPasswordMessages);
        }

        [Fact]
        public void given_invalid_confirm_password_should_show_error()
        {
            var instance = _component.Instance;
            var confirmPasswordInput = _component.Find("#confirm-password");

            confirmPasswordInput.Change("abcdefgh");

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationPasswordMessages = instance.EditContext.GetValidationMessage(() => instance.Form.ConfirmPassword);
            validationPasswordMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationPasswordMessages);
        }

        [Fact]
        public async Task given_different_password_when_sign_up_should_show_error()
        {
            var instance = _component.Instance;
            var emailInput = _component.Find("""input[type="email"]""");
            emailInput.Change("email@email.com");
            var passwordInput = _component.Find("#password");
            passwordInput.Change("P4asW0Rddt!~2a");
            var confirmPasswordInput = _component.Find("#confirm-password");
            confirmPasswordInput.Change("P4asW0Rddt!~2abca");
            var submitButton = _component.Find("""button[type="submit"]""");

            await submitButton.ClickAsync(new MouseEventArgs());

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var validationPasswordMessages = instance.EditContext.GetValidationMessage(() => instance.Form.ConfirmPassword);
            validationPasswordMessages.ShouldNotBeNullOrWhiteSpace();
            _component.Markup.ShouldContain(validationPasswordMessages);
        }

        [Fact]
        public async Task should_sign_up()
        {
            var submitButton = _component.Find("""button[type="submit"]""");
            var instance = _component.Instance;
            var emailInput = _component.Find("""input[type="email"]""");
            emailInput.Change("email@email.com");
            var passwords = _component.FindAll("""input[type="password"]""");
            passwords.Count.ShouldBe(2);
            passwords[0].Change("PaASssword!32fg");
            // after each change on DOM the collection is dirty and need to be refreshed
            // better to give some id or data attribute on html element
            passwords = _component.FindAll("""input[type="password"]""");
            passwords[1].Change("PaASssword!32fg");

            await submitButton.ClickAsync(new MouseEventArgs());

            instance.ShouldNotBeNull();
            instance.EditContext.ShouldNotBeNull();
            var errors = instance.EditContext.GetValidationMessages();
            errors.ShouldBeEmpty();
            instance.Error.ShouldBeNullOrWhiteSpace();
            await _authenticationService.Received(1).SignUpAsync(Arg.Any<SignUpDto>());
        }

        private readonly TestContext _testContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IRenderedComponent<SignUpPage> _component;

        public SignUpPageTests()
        {
            _testContext = new TestContext();
            _testContext.AddTestAuthorization();
            _authenticationService = Substitute.For<IAuthenticationService>();
            _testContext.Services.AddScoped(_ => _authenticationService);
            _component = _testContext.RenderComponent<SignUpPage>();
        }
    }
}
