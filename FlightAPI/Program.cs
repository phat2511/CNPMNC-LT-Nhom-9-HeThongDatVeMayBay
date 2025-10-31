using FlightAPI.Data.Entities;
using FlightAPI.Services;
using FlightAPI.Services.Dtos.Auth; // <-- Thêm cái này
using Microsoft.AspNetCore.Authentication.JwtBearer; // <-- Thêm cái này
using Microsoft.AspNetCore.Identity; // <-- Thêm cái này
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // <-- Thêm cái này
using System.Text; // <-- Thêm cái này
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Lấy config

// Lấy chuỗi kết nối
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký dịch vụ DbContext
builder.Services.AddDbContext<AirCloudDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(AirCloudDbContext).Assembly.FullName)));

// Đăng ký AutoMapper
// GHI CHÚ: Đảm bảo bạn có file MappingProfile và đã cài AutoMapper.Nếu không, hãy comment dòng này
builder.Services.AddAutoMapper(typeof(MappingProfile));

// =========================================================================
// === KHỐI BỊ THIẾU 1: CẤU HÌNH IDENTITY (Rất quan trọng) ===
// Dòng này đăng ký UserManager, RoleManager, v.v.
// =========================================================================
builder.Services.AddIdentity<Account, Role>(options =>
{
    // Cấu hình Password (tùy chọn)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Cấu hình User
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AirCloudDbContext>() // Bảo Identity dùng DbContext của bạn
.AddDefaultTokenProviders();


// =========================================================================
// === KHỐI BỊ THIẾU 2: CẤU HÌNH XÁC THỰC (Authentication) & JWT ===
// =========================================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Tắt nếu chạy ở local (dev)
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ValidIssuer = configuration["Jwt:Issuer"],
        // Lấy key từ appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});

// Đăng ký Phân quyền (Authorization)
builder.Services.AddAuthorization();


// Đăng ký Service mới (Tầng nghiệp vụ)
builder.Services.AddScoped<FlightAPI.Services.IFlightManagerService, FlightAPI.Services.FlightManagerService>();
builder.Services.AddScoped<IFlightInstanceService, FlightInstanceService>();
builder.Services.AddScoped<IAirportService, AirportService>();
builder.Services.AddScoped<IAirlineService, AirlineService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // Dòng này OK


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 1. Định nghĩa "cái ổ khóa" (Security Definition)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Đăng nhập xong, dán cái token (kèm 'Bearer ') vào đây. Ví dụ: \"Bearer eyJhbGciOi...\""
    });

    // 2. Bắt Swagger phải dùng cái ổ khóa này
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference // <-- Tên đúng là đây
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// =========================================================================
// === KHỐI BỊ THIẾU 3: BẬT XÁC THỰC (Phải đứng trước UseAuthorization) ===
// =========================================================================
app.UseAuthentication(); // Bật xác thực (Hỏi: "Bạn là ai?")
app.UseAuthorization(); // Bật phân quyền (Hỏi: "Bạn được làm gì?")


app.MapControllers();

app.Run();