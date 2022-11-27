using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Restaurant.IntegrationTests.Common;
using Shouldly;

namespace Restaurant.IntegrationTests.RepositoryTests
{
    public class UserRepositoryTests : BaseTest
    {
        [Fact]
        public async Task should_add_user()
        {
            var user = new User(Guid.NewGuid(), Email.Of("email@email.com"), "PASWodr123", User.Roles.UserRole, DateTime.UtcNow);

            await _userRepository.AddAsync(user);

            var userAdded = await _userRepository.GetAsync(user.Id);
            userAdded.ShouldNotBeNull();
            userAdded.Email.ShouldBe(user.Email);
            userAdded.Password.ShouldBe(userAdded.Password);
            userAdded.Role.ShouldBe(userAdded.Role);
        }
        
        [Fact]
        public async Task should_update_user()
        {
            var user = await AddDefaultUserAsync(Email.Of("email@wtemail.com"));
            user.ChangeEmail(Email.Of("emailabc123@testc.com"));
            user.ChangePassword("Pswtokra24");
            user.ChangeRole(User.Roles.AdminRole);

            await _userRepository.UpdateAsync(user);

            var userAdded = await _userRepository.GetAsync(user.Id);
            userAdded.ShouldNotBeNull();
            userAdded.Email.ShouldBe(user.Email);
            userAdded.Password.ShouldBe(userAdded.Password);
            userAdded.Role.ShouldBe(userAdded.Role);
        }

        [Fact]
        public async Task should_delete_user()
        {
            var user = await AddDefaultUserAsync(Email.Of("email@emaiaasl.com"));

            await _userRepository.DeleteAsync(user);

            var userDeleted = await _userRepository.GetAsync(user.Id);
            userDeleted.ShouldBeNull();
        }

        [Fact]
        public async Task should_get_all_users()
        {
            var user = await AddDefaultUserAsync(Email.Of("email@email124.com"));
            var user2 = await AddDefaultUserAsync(Email.Of("email@emai2aval.com"));

            var users = await _userRepository.GetAllAsync();

            users.ShouldNotBeEmpty();
            users.Count().ShouldBeGreaterThan(1);
            users.ShouldContain(u => u.Id.Equals(user.Id));
            users.ShouldContain(u => u.Id.Equals(user2.Id));
        }

        [Fact]
        public async Task should_get_user_by_email()
        {
            var user = await AddDefaultUserAsync(Email.Of("email@emailabcas.com"));

            var userAdded = await _userRepository.GetAsync(user.Email.Value);

            userAdded.ShouldNotBeNull();
            userAdded.Email.ShouldBe(user.Email);
            userAdded.Password.ShouldBe(userAdded.Password);
            userAdded.Role.ShouldBe(userAdded.Role);
        }

        private readonly IUserRepository _userRepository;

        public UserRepositoryTests(OptionsProvider optionsProvider)
            : base(optionsProvider)
        {
            _userRepository = GetRequiredService<IUserRepository>();
        }
    }
}
