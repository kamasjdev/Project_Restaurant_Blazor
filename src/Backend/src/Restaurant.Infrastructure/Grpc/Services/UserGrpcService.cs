using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Shared.UserProto;

namespace Restaurant.Infrastructure.Grpc.Services
{
	internal sealed class UserGrpcService : Users.UsersBase
	{
		private readonly IServiceProvider _serviceProvider;

		public UserGrpcService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public override async Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
			var auth = await userService.SignInAsync(new SignInDto(request.Email, request.Password));
			return new SignInResponse { AccessToken = auth.AccessToken };
		}

		public override async Task<Empty> SignUp(SignUpRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
			await userService.SignUpAsync(new SignUpDto(request.Email, request.Password, request.Role));
			return new Empty();
		}

		public override async Task<GetUsersResponse> GetUsers(Empty request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
			var users = await userService.GetAllAsync();
			var response = new GetUsersResponse();
			response.Users.Add(users.Select(u => new User
			{
				Id = u.Id.ToString(),
				Email = u.Email,
				Role = u.Role,
				CreatedAt = Timestamp.FromDateTime(u.CreatedAt)
			}));
			return response;
		}

		public override async Task<Empty> UpdateUser(UpdateUserRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
			var userId = request.UserId.AsGuid();
			await userService.UpdateAsync(new UpdateUserDto(userId, request.Email, request.Password, request.Role));
			return new Empty();
		}

		public override async Task<User> GetUser(GetUserRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
			var id = request.Id.AsGuid();
			var user = await userService.GetAsync(id);

			if (user is null)
			{
				throw new UserNotFoundException(id);
			}

			return new User
			{
				Id = user.Id.ToString(),
				Email = user.Email,
				Role = user.Role,
				CreatedAt = Timestamp.FromDateTime(user.CreatedAt)
			};
		}

		public override async Task<Empty> UpdateUserRole(UpdateUserRoleRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
			var userId = request.UserId.AsGuid();
			await userService.UpdateRoleAsync(new UpdateRoleDto(userId, request.Role));
			return new Empty();
		}

		public override async Task<Empty> ChangeUserEmail(ChangeUserEmailRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
			var userId = request.UserId.AsGuid();
			await userService.ChangeEmailAsync(new ChangeEmailDto(userId, request.Email));
			return new Empty();
		}

		public override async Task<Empty> ChangeUserPassword(ChangeUserPasswordRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
			var userId = request.UserId.AsGuid();
			await userService.ChangePasswordAsync(new ChangePasswordDto(userId, request.Password, request.NewPassword, request.NewPasswordConfirm));
			return new Empty();
		}

        public override async Task<Empty> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var userId = request.UserId.AsGuid();
            await userService.DeleteAsync(userId);
            return new Empty();
        }
    }
}
