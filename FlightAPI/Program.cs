using FlightAPI.Data.Entities; // Sử dụng namespace của Entities/Models mới
using FlightAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Lấy chuỗi kết nối
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký dịch vụ DbContext, sử dụng AirCloudDbContext
builder.Services.AddDbContext<AirCloudDbContext>(options => // <-- Dùng DbContext mới
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(AirCloudDbContext).Assembly.FullName)));

// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Đăng ký Service mới (Tầng nghiệp vụ)

builder.Services.AddScoped<FlightAPI.Services.IFlightManagerService, FlightAPI.Services.FlightManagerService>();

builder.Services.AddScoped<IFlightInstanceService, FlightInstanceService>();

builder.Services.AddScoped<IAirportService, AirportService>();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
