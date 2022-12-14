using System.Security.Claims;

namespace Restaurant.UI.UnitTests.Pages
{
    public class IndexTests
    {
        [Fact]
        public void should_render_index()
        {
            using var ctx = new TestContext();
            ctx.AddTestAuthorization();

            var component = ctx.RenderComponent<UI.Pages.Index>();

            component.Markup.Contains("Welcome to Resturant app");
        }

        [Fact]
        public void should_render_index_for_authenticated_user()
        {
            using var ctx = new TestContext();
            var authContext = ctx.AddTestAuthorization();
            var email = "email@test.com";
            authContext.SetClaims(new Claim(ClaimTypes.Email, email));

            var component = ctx.RenderComponent<UI.Pages.Index>();

            component.Markup.Contains(email);
        }
    }
}