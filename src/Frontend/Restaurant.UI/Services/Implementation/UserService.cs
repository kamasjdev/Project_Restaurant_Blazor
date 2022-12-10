using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class UserService : IUserService
    {
        private readonly IList<User> _users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Email = "admin@admin.com", Password = "NaChilku123!", CreatedAt = new DateTime(2022, 12, 5, 18, 45, 30), Role = "admin" },
            new User { Id = Guid.NewGuid(), Email = "user@user.com", Password = "NieZgadnies123!", CreatedAt = new DateTime(2022, 12, 5, 18, 45, 30), Role = "user" }
        };

        public Task ChangeEmailAsync(ChangeEmailDto changeEmailDto)
        {
            var user = _users.SingleOrDefault(u => u.Id == changeEmailDto.UserId);
            
            if (user is null)
            {
                throw new InvalidOperationException($"User with id: '{changeEmailDto.UserId}' was not found");
            }

            user.Email = changeEmailDto.Email;
            return Task.CompletedTask;
        }

        public Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = _users.SingleOrDefault(u => u.Id == changePasswordDto.UserId);

            if (user is null)
            {
                throw new InvalidOperationException($"User with id: '{changePasswordDto.UserId}' was not found");
            }

            user.Password = changePasswordDto.Password;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return Task.FromResult(_users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            }));
        }

        public Task<UserDto?> GetAsync(Guid id)
        {
            var user = _users.SingleOrDefault(u => u.Id == id);
            return Task.FromResult(user is not null ?
                new UserDto { Id = user.Id, Email = user.Email, CreatedAt = user.CreatedAt, Role = user.Role }
                : null);
        }

        public Task<AuthDto> SignInAsync(SignInDto signInDto)
        {
            var user = _users.SingleOrDefault(u => u.Email == signInDto.Email);

            if (user is null)
            {
                throw new InvalidOperationException("Invalid Credentials");
            }

            if (!string.Equals(user.Password, signInDto.Password, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException("Invalid Credentials");
            }

            return Task.FromResult(new AuthDto { AccessToken = "token" });
        }

        public Task SignUpAsync(SignUpDto signUpDto)
        {
            _users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = signUpDto.Email,
                Password = signUpDto.Password,
                CreatedAt = DateTime.UtcNow,
                Role = string.IsNullOrWhiteSpace(signUpDto.Role) ? "user" : signUpDto.Role
            });
            return Task.CompletedTask;
        }

        public Task UpdateAsync(UpdateUserDto updateUserDto)
        {
            return Task.CompletedTask;
        }

        public Task UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            var user = _users.SingleOrDefault(u => u.Id == updateRoleDto.UserId);

            if (user is null)
            {
                throw new InvalidOperationException($"User with id: '{updateRoleDto.UserId}' was not found");
            }

            user.Role = updateRoleDto.Role;
            return Task.CompletedTask;
        }

        public User? GetAsync(string email) => _users.SingleOrDefault(u => u.Email == email);

        public Task DeleteAsync(Guid id)
        {
            var user = _users.SingleOrDefault(u => u.Id == id);

            if (user is null)
            {
                throw new InvalidOperationException($"User with id: '{id}' was not found");
            }

            _users.Remove(user);
            return Task.CompletedTask;
        }

        internal class User
        {
            public Guid Id { get; set; }
            public string? Email { get; set; }
            public string? Role { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? Password { get; set; }
        }
    }
}
