using Blazored.LocalStorage;
using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IList<User> _users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Email = "admin@admin.com", Password = "NaChilku123!", CreatedAt = new DateTime(2022, 12, 5, 18, 45, 30), Role = "admin" },
            new User { Id = Guid.NewGuid(), Email = "user@user.com", Password = "NieZgadnies123!", CreatedAt = new DateTime(2022, 12, 5, 18, 45, 30), Role = "user" }
        };
        private const string TOKEN = "token";

        public AuthenticationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<UserDto> SignInAsync(SignInDto signInDto)
        {
            using var scope = _serviceProvider.CreateScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            var user = _users.SingleOrDefault(u => u.Email == signInDto.Email);

            if (user is null)
            {
                throw new InvalidOperationException("Invalid Credentials");
            }

            if (!string.Equals(user.Password, signInDto.Password, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException("Invalid Credentials");
            }

            var userDto = new UserDto { Id = user.Id, Email = user.Email, CreatedAt = user.CreatedAt, Role = user.Role };
            await localStorageService.SetItemAsync("token", userDto);
            return userDto;
        }

        public async Task SignoutAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            await localStorageService.RemoveItemAsync(TOKEN);
        }

        public Task SignUpAsync(SignUpDto signUpDto)
        {
            _users.Add(new User { Id = Guid.NewGuid(), Email = signUpDto.Email, Password = signUpDto.Password,
                CreatedAt = DateTime.UtcNow, Role = string.IsNullOrWhiteSpace(signUpDto.Role) ? "user" : signUpDto.Role });
            return Task.CompletedTask;
        }

        private class User
        {
            public Guid Id { get; set; }
            public string? Email { get; set; }
            public string? Role { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? Password { get; set; }
        }
    }    
}
