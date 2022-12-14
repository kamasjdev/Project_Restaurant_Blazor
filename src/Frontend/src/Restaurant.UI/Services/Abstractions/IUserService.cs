using Restaurant.UI.DTO;

namespace Restaurant.UI.Services.Abstractions
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetAsync(Guid id);
        Task SignUpAsync(SignUpDto signUpDto);
        Task<AuthDto> SignInAsync(SignInDto signInDto);
        Task UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task ChangeEmailAsync(ChangeEmailDto changeEmailDto);
        Task ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task UpdateAsync(UpdateUserDto updateUserDto);
        Task DeleteAsync(Guid id);
    }
}
