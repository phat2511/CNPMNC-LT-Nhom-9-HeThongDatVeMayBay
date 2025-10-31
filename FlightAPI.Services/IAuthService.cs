// 1. Sửa using cho đúng đường dẫn mới
using FlightAPI.Services.Dtos.Auth;

namespace FlightAPI.Services
{
    // 2. Sửa "internal" thành "public"
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    }
}