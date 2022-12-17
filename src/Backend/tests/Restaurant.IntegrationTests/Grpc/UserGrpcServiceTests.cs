using Google.Protobuf.WellKnownTypes;
using Restaurant.IntegrationTests.Common;
using Restaurant.Shared.UserProto;
using Shouldly;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Restaurant.IntegrationTests.Grpc
{
    public class UserGrpcServiceTests : GrpcTestBase
    {
        [Fact]
        public async Task should_sign_in()
        {
            var user = TestData.GetAdminUser();
            var currentDateTime = DateTime.UtcNow;

            var response = await _client.SignInAsync(new SignInRequest { Email = user.Email.Value, Password = user.Password });

            response.ShouldNotBeNull();
            response.AccessToken.ShouldNotBeNullOrWhiteSpace();
            response.AccessToken.ShouldContain('.');
            var claims = TestExtensions.ParseClaimsFromJwt(response.AccessToken);
            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            emailClaim.ShouldNotBeNull();
            emailClaim.Value.ShouldBe(user.Email.Value);
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            roleClaim.ShouldNotBeNull();
            roleClaim.Value.ShouldBe(user.Role);
            var idClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            idClaim.ShouldNotBeNull();
            idClaim.Value.ShouldNotBeNullOrWhiteSpace();
            Guid.Parse(idClaim.Value);
            var expireClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            expireClaim.ShouldNotBeNull();
            DateTime expireDate = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            expireDate = expireDate.AddSeconds(long.Parse(expireClaim.Value));
            expireDate.ShouldBeGreaterThan(currentDateTime);
            expireDate.ShouldBeLessThanOrEqualTo(currentDateTime.AddHours(1));
        }

        [Fact]
        public async Task should_sign_up()
        {
            var request = new SignUpRequest { Email = "test-email@abc123ab2.com", Password = "Pswafqrqwsxdgvsdga1331!", Role = "admin" };

            var respnse = await _client.SignUpAsync(request);

            respnse.ShouldNotBeNull();
        }

        [Fact]
        public async Task should_sign_up_and_sign_in()
        {
            var request = new SignUpRequest { Email = "test123-email@abc123ab2.com", Password = "Pswafqrqwsxdgvsdga1331!", Role = "admin" };

            await _client.SignUpAsync(request);
            var response = await _client.SignInAsync(new SignInRequest { Email = request.Email, Password = request.Password });

            response.ShouldNotBeNull();
            response.AccessToken.ShouldNotBeNullOrWhiteSpace();
            response.AccessToken.ShouldContain('.');
        }

        [Fact]
        public async Task should_change_email()
        {
            var user = await AddDefaultUser();
            var responseSignIn = await _client.SignInAsync(new SignInRequest { Email = user.Email, Password = user.Password });
            var id = GetIdFromToken(responseSignIn.AccessToken);
            var uniqueEmail = "uniqueEmail1q2@unieq.ee.com";

            await _client.ChangeUserEmailAsync(new ChangeUserEmailRequest { Email = uniqueEmail, UserId = id });

            var userEmailChanged = await _client.GetUserAsync(new GetUserRequest { Id = id });
            userEmailChanged.Email.ShouldBe(uniqueEmail);
        }

        [Fact]
        public async Task should_change_password()
        {
            var user = await AddDefaultUser();
            var responseSignIn = await _client.SignInAsync(new SignInRequest { Email = user.Email, Password = user.Password });
            var id = GetIdFromToken(responseSignIn.AccessToken);
            var password = "abcmasfasgfio@!asfa!aasfSFA12";

            await _client.ChangeUserPasswordAsync(new ChangeUserPasswordRequest { UserId = id, NewPassword = password, NewPasswordConfirm = password, Password = user.Password });

            var response = await _client.SignInAsync(new SignInRequest { Email = user.Email, Password = password });
            response.ShouldNotBeNull();
            response.AccessToken.ShouldNotBeNullOrWhiteSpace();
            response.AccessToken.ShouldContain('.');
            GetIdFromToken(response.AccessToken).ShouldBe(id);
        }

        [Fact]
        public async Task should_change_role()
        {
            var user = await AddDefaultUser();
            var responseSignIn = await _client.SignInAsync(new SignInRequest { Email = user.Email, Password = user.Password });
            var id = GetIdFromToken(responseSignIn.AccessToken);
            var role = "admin";

            await _client.UpdateUserRoleAsync(new UpdateUserRoleRequest { Role = role, UserId = id });

            var userRoleChanged = await _client.GetUserAsync(new GetUserRequest { Id = id });
            userRoleChanged.Role.ShouldBe(role);
        }

        [Fact]
        public async Task should_update_user()
        {
            var user = await AddDefaultUser();
            var responseSignIn = await _client.SignInAsync(new SignInRequest { Email = user.Email, Password = user.Password });
            var id = GetIdFromToken(responseSignIn.AccessToken);
            var role = "admin";
            var password = "abcmasfasgfio@!asfa!aasfSFA12";
            var uniqueEmail = "uniqueEmailll1q2@unieq.ee.com";

            await _client.UpdateUserAsync(new UpdateUserRequest { Email = uniqueEmail, Password = password, Role = role, UserId = id });

            var response = await _client.SignInAsync(new SignInRequest { Email = uniqueEmail, Password = password });
            response.ShouldNotBeNull();
            response.AccessToken.ShouldNotBeNullOrWhiteSpace();
            response.AccessToken.ShouldContain('.');
            var claims = TestExtensions.ParseClaimsFromJwt(response.AccessToken);
            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            emailClaim.ShouldNotBeNull();
            emailClaim.Value.ShouldBe(uniqueEmail);
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            roleClaim.ShouldNotBeNull();
            roleClaim.Value.ShouldBe(role);
            var idClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            idClaim.ShouldNotBeNull();
            idClaim.Value.ShouldNotBeNullOrWhiteSpace();
            idClaim.Value.ShouldBe(id);
        }

        [Fact]
        public async Task should_get_user()
        {
            var user = TestData.GetAdminUser();
            var response = await _client.SignInAsync(new SignInRequest { Email = user.Email.Value, Password = user.Password });
            var id = GetIdFromToken(response.AccessToken);

            var responseGetUser = await _client.GetUserAsync(new GetUserRequest { Id = id });

            responseGetUser.ShouldNotBeNull();
            responseGetUser.Id.ShouldBe(id);
            responseGetUser.Email.ShouldBe(user.Email.Value);
            responseGetUser.Role.ShouldBe(user.Role);
        }

        [Fact]
        public async Task should_get_users()
        {
            var user1 = await AddDefaultUser();
            var user2 = await AddDefaultUser();

            var response = await _client.GetUsersAsync(new Empty());

            response.ShouldNotBeNull();
            response.Users.ShouldNotBeEmpty();
            response.Users.Count.ShouldBeGreaterThan(1);
            response.Users.ShouldContain(u => u.Email == user1.Email);
            response.Users.ShouldContain(u => u.Email == user2.Email);
        }

        private async Task<TestUserAdded> AddDefaultUser()
        {
            var request = new SignUpRequest { Email = $"{Guid.NewGuid().ToString("N")}@{Guid.NewGuid().ToString("N")}.com", Password = "Pswafqrqwsxdgvsdga1331!", Role = "user" };
            await _client.SignUpAsync(request);
            return new TestUserAdded(request.Email, request.Password);
        }

        private string GetIdFromToken(string token)
        {
            var claims = TestExtensions.ParseClaimsFromJwt(token);
            string id = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? "00000000-0000-0000-0000-000000000000";
            return id;
        }

        private record TestUserAdded(string Email, string Password);

        private readonly Users.UsersClient _client;

        public UserGrpcServiceTests(OptionsProvider optionsProvider)
            : base(optionsProvider)
        {
            _client = new Users.UsersClient(Channel);
        }
    }
}
