using Restaurant.Shared.UserProto;
using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class UserService : IUserService
    {
        private readonly Users.UsersClient _usersClient;

        public UserService(Users.UsersClient usersClient)
        {
            _usersClient = usersClient;
        }

        public async Task ChangeEmailAsync(ChangeEmailDto changeEmailDto)
        {
            await _usersClient.ChangeUserEmailAsync(new ChangeUserEmailRequest
            {
                UserId = changeEmailDto.UserId.ToString(),
                Email = changeEmailDto.Email
            });
        }

        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            await _usersClient.ChangeUserPasswordAsync(new ChangeUserPasswordRequest
            {
                UserId = changePasswordDto.UserId.ToString(),
                Password = changePasswordDto.Password,
                NewPassword = changePasswordDto.NewPassword,
                NewPasswordConfirm = changePasswordDto.NewPasswordConfirm
            });
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return (await _usersClient.GetUsersAsync(new Google.Protobuf.WellKnownTypes.Empty()))
                .Users.Select(u => new UserDto
                {
                    Id = Guid.Parse(u.Id),
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt.ToDateTime()
                });
        }

        public async Task<UserDto?> GetAsync(Guid id)
        {
            var user = await _usersClient.GetUserAsync(new GetUserRequest
            {
                Id = id.ToString()
            });
            return user is not null ? new UserDto
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt.ToDateTime()
            } : null;
        }

        public async Task<AuthDto> SignInAsync(SignInDto signInDto)
        {
            var auth = await _usersClient.SignInAsync(new SignInRequest
            {
                Email = signInDto.Email,
                Password = signInDto.Password
            });
            return new AuthDto { AccessToken = auth.AccessToken };
        }

        public async Task SignUpAsync(SignUpDto signUpDto)
        {
            var request = new SignUpRequest
            {
                Email = signUpDto.Email,
                Password = signUpDto.Password
            };
            if (!string.IsNullOrWhiteSpace(signUpDto.Role))
            {
                request.Role = signUpDto.Role;
            }
            await _usersClient.SignUpAsync(request);
        }

        public async Task UpdateAsync(UpdateUserDto updateUserDto)
        {
            await _usersClient.UpdateUserAsync(new UpdateUserRequest
            {
                UserId = updateUserDto.UserId.ToString(),
                Email = updateUserDto.Email,
                Password = updateUserDto.Password,
                Role = updateUserDto.Role
            });
        }

        public async Task UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            await _usersClient.UpdateUserRoleAsync(new UpdateUserRoleRequest
            {
                UserId = updateRoleDto.UserId.ToString(),
                Role = updateRoleDto.Role
            });
        }

        public async Task DeleteAsync(Guid id)
        {
            // TODO: _usersClient.DeleteUserAsync(id)
            return;
        }
    }
}
