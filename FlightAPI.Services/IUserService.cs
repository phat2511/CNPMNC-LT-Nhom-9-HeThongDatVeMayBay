using FlightAPI.Services.Dtos.User;

namespace FlightAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDetailDto>> GetAllAsync();
        Task ToggleLockAsync(int userId, UserLockRequestDto dto);
        Task ChangeRoleAsync(int userId, UserRoleUpdateDto dto);
    }
}