
using FlightAPI.Data;
using Microsoft.Extensions.Logging;
using FlightAPI.Data.Entities;
using FlightAPI.Services.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System; // (Cho Random và Exception)
using System.Collections.Generic; // (Cho List)
using System.IdentityModel.Tokens.Jwt;
using System.Linq; // (Cho .Select)
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks; // (Cho Task)

namespace FlightAPI.Services
{
    // "Não" xịn, "thề" (implement) 3 "hợp đồng" mới
    public class AuthService : IAuthService
    {
        // "Não" này cần 3 "đồ nghề"
        private readonly UserManager<Account> _userManager;
        private readonly AirCloudDbContext _context;
        private readonly IConfiguration _configuration;

        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;


        // "Tiêm" (Inject) 3 "đồ nghề"
        public AuthService(
    UserManager<Account> userManager,
    AirCloudDbContext context,
    IConfiguration configuration,
    ILogger<AuthService> logger,
            IEmailService emailService) // <-- HÀNG MỚI
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _logger = logger; // <-- GÁN HÀNG MỚI

            _emailService = emailService;
        }

        // =======================================================
        // "NÃO" 1: PHẪU THUẬT REGISTER
        // =======================================================
        public async Task RegisterAsync(RegisterRequestDto dto)
        {
            // (Check trùng User/Email)
            var existingUser = await _userManager.FindByNameAsync(dto.Username);
            if (existingUser != null) throw new Exception("Username đã tồn tại");
            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null) throw new Exception("Email đã tồn tại");

            // Tìm Role "User"
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole == null) throw new Exception("Hệ thống chưa có Role 'User'. Vui lòng seed data.");

            var newUser = new Account
            {
                UserName = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                RoleId = userRole.Id,
                // === KHÓA ===
                EmailConfirmed = false, // (Vì 'IsEmailConfirmed' map với 'EmailConfirmed')
                IsLocked = false
            };

            // "Tạo" user (với pass đã "hash")
            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Tạo tài khoản thất bại: {errors}");
            }

            // === "NÂNG CẤP": GỬI MÃ KÍCH HOẠT ===
            await SendVerificationEmail(newUser);
        }

        // =======================================================
        // "NÃO" 2: "CỬA" MỚI (VERIFY EMAIL)
        // =======================================================
        public async Task VerifyEmailAsync(VerifyEmailRequestDto dto)
        {
            // 1. Tìm "thằng" user
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) throw new Exception("Email không tồn tại.");

            // 2. Tìm "mã" (code) "xịn"
            var verification = await _context.EmailVerifications
                .FirstOrDefaultAsync(v =>
                    v.AccountId == user.Id &&
                    v.Code == dto.Code &&
                    v.IsUsed == false); // (Chưa "xài")

            if (verification == null)
            {
                throw new Exception("Mã kích hoạt không hợp lệ.");
            }

            // 3. Check "hạn" (Expired)
            if (verification.ExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Mã kích hoạt đã hết hạn.");
            }

            // 4. "CHỐT ĐƠN": Kích hoạt
            user.EmailConfirmed = true; // (Kích hoạt "Account")
            verification.IsUsed = true; // (Vô hiệu hóa "Mã")

            // (Lưu "cả hai" thay đổi)
            await _context.SaveChangesAsync();
        }

        // =======================================================
        // "NÃO" 3: PHẪU THUẬT LOGIN
        // =======================================================
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            // (Chốt 1: Check user)
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null) throw new Exception("Sai Username hoặc Password");

            // (Chốt 2: Check pass)
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid) throw new Exception("Sai Username hoặc Password");

            // === "VÁ" (PATCH) 1: CHECK "KHÓA" (của Admin) ===
            if (user.IsLocked == true)
            {
                throw new Exception($"Tài khoản đã bị khóa. Lý do: {user.LockReason ?? "Không lý do"}");
            }

            // === "VÁ" (PATCH) 2: CHECK "KÍCH HOẠT" (Email) ===
            if (user.EmailConfirmed == false)
            {
                throw new Exception("Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email.");
            }

            // (Lấy Role "chuẩn" - không cần RoleManager)
            var userWithRole = await _context.Users
                                        .Include(u => u.Role) // "JOIN"
                                        .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithRole?.Role == null)
            {
                throw new Exception("Lỗi hệ thống: User không có Role.");
            }

            var roles = new List<string> { userWithRole.Role.Name };
            var token = GenerateJwtToken(user, roles);

            return new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Username = user.UserName,
                Roles = roles
            };
        }

        // =======================================================
        // --- "HÀM" (METHOD) "NỘI BỘ" (PRIVATE) ---
        // =======================================================

        // (Hàm "gửi" mail "giả lập")
        private async Task SendVerificationEmail(Account user)
        {
            // 1. "Tạo" mã 6 số (Random)
            var code = new Random().Next(100000, 999999).ToString();

            // 2. "Đúc" cái "vé" (Entity)
            var verification = new EmailVerification
            {
                AccountId = user.Id,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10), // "Hạn" 10 phút
                IsUsed = false
            };

            // 3. "Nhét" vào "kho"
            _context.EmailVerifications.Add(verification);
            await _context.SaveChangesAsync();

            try
            {
                var subject = "Chào mừng đến AirCloud! Kích hoạt tài khoản.";
                var htmlContent = $@"
        <h1>Cảm ơn sếp đã đăng ký!</h1>
        <p>Mã kích hoạt (OTP) của sếp là:</p>
        <h2 style='color: blue;'>{code}</h2>
        <p>Mã này sẽ hết hạn trong 10 phút.</p>";

                // "Alo, Bưu tá (EmailService)? Gửi cái này đi!"
                await _emailService.SendEmailAsync(user.Email, subject, htmlContent);
            }
            catch (Exception ex)
            {
                // (Nếu "bưu tá" (Brevo) "chết" (fail) -> "thằng" 'EmailService' "xịn"
                //  nó "ném" (throws) lỗi ra, "não" Auth "bắt" (catches) được ở đây)
                _logger.LogError(ex, "LỖI BƯU TÁ: Không gửi được email kích hoạt cho {Email}", user.Email);

                // (Sếp "vẫn" (still) "in" (print) ra "Tủ" (console) để "test" (test)
                //  phòng hờ (in case) "bưu tá" "chết" (fail) hoặc "chìa" (key) "hết hạn" (expires))
                _logger.LogInformation("===== EMAIL KÍCH HOẠT (Phòng hờ) =====");
                _logger.LogInformation("Gửi đến: {Email}", user.Email);
                _logger.LogInformation("Mã (Backup): {Code}", code);
                _logger.LogInformation("=====================================");
            }
        }

        // (Hàm "in" token "chuẩn")
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
}