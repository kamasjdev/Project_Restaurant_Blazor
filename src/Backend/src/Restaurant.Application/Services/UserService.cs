using Microsoft.Extensions.Logging;
using Restarant.Application.Mappings;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Application.Services
{
    internal sealed class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordManager _passwordManager;
        private readonly IJwtManager _jwtManager;
        private readonly IClock _clock;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IPasswordManager passwordManager, IJwtManager jwtManager, IClock clock,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _passwordManager = passwordManager;
            _jwtManager = jwtManager;
            _clock = clock;
            _logger = logger;
        }

        public async Task ChangeEmailAsync(ChangeEmailDto changeEmailDto)
        {
            var user = await _userRepository.GetAsync(changeEmailDto.UserId);

            if (user is null)
            {
                throw new UserNotFoundException(changeEmailDto.UserId);
            }

            var userExists = await _userRepository.GetAsync(changeEmailDto.Email);

            if (userExists is not null)
            {
                _logger.LogError($"Invalid change email operation. Cannot change email for user with id: '{user.Id}' to '{changeEmailDto.Email}', probably is expropriating the data");
                return;
            }

            user.ChangeEmail(Email.Of(changeEmailDto.Email));
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetAsync(changePasswordDto.UserId);

            if (user is null)
            {
                throw new UserNotFoundException(changePasswordDto.UserId);
            }

            var password = new Password(changePasswordDto.Password);
            var newPassword = new Password(changePasswordDto.NewPassword);
            var newPasswordConfirm = new Password(changePasswordDto.NewPasswordConfirm);

            if (!newPassword.Equals(newPasswordConfirm))
            {
                throw new NewPasswordsAreNotSameException();
            }

            if (!_passwordManager.Validate(password.Value, user.Password))
            {
                throw new InvalidCredentialsException();
            }

            if (password.Equals(newPassword.Value))
            {
                return;
            }

            var securedPassword = _passwordManager.Secure(changePasswordDto.NewPassword);
            user.ChangePassword(securedPassword);
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);

            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }

            await _userRepository.DeleteAsync(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return (await _userRepository.GetAllAsync()).Select(u => u.AsDto());
        }

        public async Task<UserDto> GetAsync(Guid id)
        {
            return (await _userRepository.GetAsync(id)).AsDto();
        }

        public async Task<AuthDto> SignInAsync(SignInDto signInDto)
        {
            var user = await _userRepository.GetAsync(signInDto.Email);
            if (user is null)
            {
                throw new InvalidCredentialsException();
            }

            if (!_passwordManager.Validate(signInDto.Password, user.Password))
            {
                throw new InvalidCredentialsException();
            }

            var token = _jwtManager.CreateToken(user.Id, user.Role);

            return new AuthDto
            {
                AccessToken = token
            };
        }

        public async Task SignUpAsync(SignUpDto signUpDto)
        {
            var email = new Email(signUpDto.Email);
            var password = new Password(signUpDto.Password);
            var role = string.IsNullOrWhiteSpace(signUpDto.Role) ? User.Roles.UserRole : signUpDto.Role;

            if (await _userRepository.GetAsync(email.Value) is not null)
            {
                throw new EmailAlreadyInUseException(email.Value);
            }

            var securedPassword = _passwordManager.Secure(password);
            var user = User.Create(email, securedPassword, role, _clock.CurrentDate());
            await _userRepository.AddAsync(user);
        }

        public async Task UpdateAsync(UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetAsync(updateUserDto.UserId);

            if (user is null)
            {
                throw new UserNotFoundException(updateUserDto.UserId);
            }

            user.ChangeEmail(Email.Of(updateUserDto.Email));
            user.ChangeRole(updateUserDto.Role);
            var password = new Password(updateUserDto.Password);
            var securedPassword = _passwordManager.Secure(password);
            user.ChangePassword(securedPassword);
            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            var user = await _userRepository.GetAsync(updateRoleDto.UserId);

            if (user is null)
            {
                throw new UserNotFoundException(updateRoleDto.UserId);
            }

            user.ChangeRole(updateRoleDto.Role);
            await _userRepository.UpdateAsync(user);
        }
    }
}
