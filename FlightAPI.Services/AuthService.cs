using FlightAPI.Data.Entities;
using FlightAPI.Services.Dtos.Auth; // Đảm bảo using DTOs
using FlightAPI.Data; // <-- THÊM CÁI NÀY
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore; // <-- THÊM CÁI NÀY
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FlightAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<Account> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly AirCloudDbContext _context; // <-- SỬA 1: Thêm DbContext

    public AuthService(
        UserManager<Account> userManager,
        RoleManager<Role> roleManager,
        IConfiguration configuration,
        AirCloudDbContext context) // <-- SỬA 2: Inject DbContext
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _context = context; // <-- SỬA 3: Gán DbContext
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var existingUser = await _userManager.FindByNameAsync(dto.Username);
        if (existingUser != null)
        {
            throw new Exception("Username đã tồn tại");
        }

        var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existingEmail != null)
        {
            throw new Exception("Email đã tồn tại");
        }

        // === SỬA 4: TÌM ROLE ID TRƯỚC ===
        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (userRole == null)
        {
            // Nếu bạn chưa seed data, nó sẽ lỗi ở đây.
            // Chạy: INSERT INTO auth.Role (RoleName) VALUES ('User')
            throw new Exception("Hệ thống chưa có Role 'User'. Vui lòng seed data.");
        }

        // === SỬA 5: TẠO USER VỚI ROLEID ===
        var newUser = new Account
        {
            UserName = dto.Username,
            Email = dto.Email,
            FullName = dto.FullName,
            CreatedAt = DateTime.UtcNow,
            RoleId = userRole.Id // <-- GÁN ROLE ID TRỰC TIẾP
        };

        var result = await _userManager.CreateAsync(newUser, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Tạo tài khoản thất bại: {errors}");
        }

        // === SỬA 6: XOÁ LOGIC CŨ ===
        // Dòng này dùng bảng UserRoles, chúng ta không dùng
        // await _userManager.AddToRoleAsync(newUser, "User"); 

        // 7. Đăng ký xong thì đăng nhập luôn
        return await LoginAsync(new LoginRequestDto { Username = dto.Username, Password = dto.Password });
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null)
        {
            throw new Exception("Sai Username hoặc Password");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            throw new Exception("Sai Username hoặc Password");
        }

        if (user.IsLocked == true)
        {
            // "Vệ sĩ" "đuổi"
            throw new Exception($"Tài khoản đã bị khóa bởi Admin. Lý do: {user.LockReason ?? "Không có lý do"}");
        }

        // Lấy role từ DbContext (thay vì GetRolesAsync)
        var userWithRole = await _context.Users
                                    .Include(u => u.Role) // Tải Role
                                    .FirstOrDefaultAsync(u => u.Id == user.Id);

        var roles = new List<string> { userWithRole.Role.Name }; // Lấy tên Role

        var token = GenerateJwtToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            FullName = user.FullName,
            Username = user.UserName,
            Roles = roles
        };
    }

    // Hàm GenerateJwtToken (không đổi, giữ nguyên)
    private string GenerateJwtToken(Account user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("fullname", user.FullName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}