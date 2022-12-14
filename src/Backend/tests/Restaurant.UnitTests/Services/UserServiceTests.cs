using Microsoft.Extensions.Logging;
using NSubstitute;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Services;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task should_sign_up()
        {
            var dto = new SignUpDto("email@test.com", "PasWord!@2141", User.Roles.AdminRole);

            await _userService.SignUpAsync(dto);

            await _userRepository.Received(1).AddAsync(Arg.Any<User>());
            _passwordManager.Received(1).Secure(dto.Password);
        }

        [Fact]
        public async Task given_existing_email_when_sign_up_should_throw_an_exception()
        {
            var user = AddDefaultUser();
            var dto = new SignUpDto(user.Email.Value, "PasWord!@2141", User.Roles.AdminRole);
            var expectedException = new EmailAlreadyInUseException(dto.Email);

            var exception = await Record.ExceptionAsync(() => _userService.SignUpAsync(dto));

            exception.GetType().ShouldBe(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((EmailAlreadyInUseException)exception).Email.ShouldBe(expectedException.Email);
        }

        [Fact]
        public async Task should_sign_in()
        {
            var user = AddDefaultUser();
            _passwordManager.Validate(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            var dto = new SignInDto(user.Email.Value, user.Password);

            await _userService.SignInAsync(dto);

            await _userRepository.Received(1).GetAsync(dto.Email);
            _jwtManager.Received(1).CreateToken(user.Id, user.Role,user.Email.Value);
        }

        [Theory]
        [InlineData("email-test@test.com")]
        [InlineData("email@email.com")]
        public async Task given_invalid_credentials_when_sign_in_should_throw_an_exception(string email)
        {
            var user = AddDefaultUser();
            var dto = new SignInDto(email, "ac");
            var expectedException = new InvalidCredentialsException();

            var exception = await Record.ExceptionAsync(() => _userService.SignInAsync(dto));

            exception.GetType().ShouldBe(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public async Task should_change_email()
        {
            var user = AddDefaultUser();
            var dto = new ChangeEmailDto(user.Id, user.Email.Value + "abc");

            await _userService.ChangeEmailAsync(dto);

            await _userRepository.Received(1).UpdateAsync(user);
        }

        [Fact]
        public async Task given_not_existing_user_when_change_email_should_throw_an_exception()
        {
            var dto = new ChangeEmailDto(Guid.NewGuid(), "email@email.com");
            var expectedException = new UserNotFoundException(dto.UserId);

            var exception = await Record.ExceptionAsync(() => _userService.ChangeEmailAsync(dto));

            exception.GetType().ShouldBe(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((UserNotFoundException)exception).UserId.ShouldBe(expectedException.UserId);
        }

        [Fact]
        public async Task given_existing_new_email_shouldnt_update_user()
        {
            var user = AddDefaultUser();
            var dto = new ChangeEmailDto(user.Id, "user@euser.com");
            AddDefaultUser(dto.Email);

            await _userService.ChangeEmailAsync(dto);

            await _userRepository.DidNotReceive().UpdateAsync(user);
        }

        [Fact]
        public async Task should_change_password()
        {
            var user = AddDefaultUser();
            _passwordManager.Validate(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            var dto = new ChangePasswordDto(user.Id, user.Password, "Pasw!2Avxa552erafsf", "Pasw!2Avxa552erafsf");

            await _userService.ChangePasswordAsync(dto);

            _passwordManager.Received(1).Secure(Arg.Any<string>());
            await _userRepository.Received(1).UpdateAsync(user);
        }

        [Fact]
        public async Task given_not_existing_user_when_change_password_should_trow_an_exception()
        {
            var user = AddDefaultUser();
            var userId = Guid.NewGuid();
            var dto = new ChangePasswordDto(userId, user.Email.Value, "newPasWo0RD1!21", "newPasWo0RD1!21");
            var expectedException = new UserNotFoundException(dto.UserId);

            var exception = await Record.ExceptionAsync(() => _userService.ChangePasswordAsync(dto));

            exception.GetType().ShouldBe(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((UserNotFoundException)exception).UserId.ShouldBe(expectedException.UserId);
        }

        [Fact]
        public async Task given_not_same_new_passwords_when_change_password_should_throw_an_exception()
        {
            var user = AddDefaultUser();
            var expectedException = new NewPasswordsAreNotSameException();
            var dto = new ChangePasswordDto(user.Id, "WAbcasw2asf@as!asf.com", "Ne3WdPasf!wsord", "Ne3WdPasf!wsordavc");

            var exception = await Record.ExceptionAsync(() => _userService.ChangePasswordAsync(dto));

            exception.GetType().ShouldBe(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public async Task given_same_new_password_as_existing_one_shouldnt_change_password()
        {
            var user = AddDefaultUser();
            _passwordManager.Validate(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            var dto = new ChangePasswordDto(user.Id, user.Password, user.Password, user.Password);

            await _userService.ChangePasswordAsync(dto);

            await _userRepository.DidNotReceive().UpdateAsync(user);
        }

        [Fact]
        public async Task should_update_user()
        {
            var user = AddDefaultUser();
            var dto = new UpdateUserDto(user.Id, user.Email.Value, "Abgaawq231asfasf", User.Roles.UserRole);

            await _userService.UpdateAsync(dto);

            await _userRepository.Received(1).UpdateAsync(user);
        }

        [Fact]
        public async Task given_not_existing_user_when_update_should_throw_an_exception()
        {
            var user = AddDefaultUser();
            var userId = Guid.NewGuid();
            var expectedException = new UserNotFoundException(userId);
            var dto = new UpdateUserDto(userId, user.Email.Value, "Abgaawq231asfasf", User.Roles.UserRole);

            var exception = await Record.ExceptionAsync(() => _userService.UpdateAsync(dto));

            exception.GetType().ShouldBe(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((UserNotFoundException)exception).UserId.ShouldBe(expectedException.UserId);
        }

        [Fact]
        public async Task should_update_role()
        {
            var user = AddDefaultUser();
            var dto = new UpdateRoleDto(user.Id, User.Roles.AdminRole);

            await _userService.UpdateRoleAsync(dto);

            await _userRepository.Received(1).UpdateAsync(user);
        }

        [Fact]
        public async Task given_not_existing_user_when_update_role_should_throw_an_exception()
        {
            var user = AddDefaultUser();
            var userId = Guid.NewGuid();
            var dto = new UpdateRoleDto(userId, User.Roles.AdminRole);
            var expectedException = new UserNotFoundException(userId);

            var exception = await Record.ExceptionAsync(() => _userService.UpdateRoleAsync(dto));

            exception.GetType().ShouldBe(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((UserNotFoundException)exception).UserId.ShouldBe(expectedException.UserId);
        }

        private User AddDefaultUser(string? email = null)
        {
            var user = new User(Guid.NewGuid(), Email.Of(email ?? "email@email.com"), "PAsWaa21asdAVxxz!9Rd", User.Roles.UserRole, _clock.CurrentDate());
            _userRepository.GetAsync(user.Id).Returns(user);
            _userRepository.GetAsync(user.Email.Value).Returns(user);
            return user;
        }

        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordManager _passwordManager;
        private readonly IJwtManager _jwtManager;
        private readonly IClock _clock;

        public UserServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _passwordManager = Substitute.For<IPasswordManager>();
            _jwtManager = Substitute.For<IJwtManager>();
            _clock = Substitute.For<IClock>();
            _clock.CurrentDate().Returns(new DateTime(2022, 9, 13, 17, 55, 10));
            var logger = Substitute.For<ILogger<UserService>>();
            _userService = new UserService(_userRepository, _passwordManager, _jwtManager, _clock, logger);
        }
    }
}
