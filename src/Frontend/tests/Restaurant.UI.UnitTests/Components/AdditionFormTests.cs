using Microsoft.AspNetCore.Components.Web;
using Restaurant.UI.Components.Additions;
using Restaurant.UI.DTO;

namespace Restaurant.UI.UnitTests.Components
{
    public class AdditionFormTests
    {
        [Fact]
        public void should_render_addition_form()
        {
            _component.Markup.ShouldContain("Name");
        }

        [Theory]
        [InlineData("")]
        [InlineData("      ")]
        [InlineData("ad")]
        public void given_invalid_addition_name_should_show_error(string additionName)
        {
            AdditionDto? addition = null;
            _component = _testContext.RenderComponent<AdditionFormComponent>(parameters =>
            parameters.Add(p => p.OnSend, (a) => addition = a));
            var additionNameInput = _component.Find("#AdditionName");

            additionNameInput.Change(additionName);

            var errors = _component.FindAll(".invalid-feedback.d-flex.text-start");
            errors.ShouldNotBeEmpty();
        }

        [Fact]
        public void given_invalid_price_should_show_error()
        {
            var price = -10;
            var priceInput = _component.Find("#Price");

            priceInput.Change(price);

            var errors = _component.FindAll(".invalid-feedback.d-flex.text-start");
            errors.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task should_send_addition_form()
        {
            AdditionDto? addition = null;
            _component = _testContext.RenderComponent<AdditionFormComponent>(parameters =>
            parameters.Add(p => p.OnSend, (a) => addition = a));
            var additionName = "Addition";
            var additionNameInput = _component.Find("#AdditionName");
            additionNameInput.Change(additionName);
            var price = 10;
            var priceInput = _component.Find("#Price");
            priceInput.Change(price);
            var sendButton = _component.Find(".btn.btn-success.me-2");

            await sendButton.ClickAsync(new MouseEventArgs());

            var errors = _component.FindAll(".invalid-feedback.d-flex.text-start");
            errors.ShouldBeEmpty();
            addition.ShouldNotBeNull();
            addition.AdditionName.ShouldBe(additionName);
            addition.Price.ShouldBe(price);
        }

        private readonly TestContext _testContext;
        private IRenderedComponent<AdditionFormComponent> _component;

        public AdditionFormTests()
        {
            _testContext = new TestContext();
            _component = _testContext.RenderComponent<AdditionFormComponent>();
        }
    }
}
