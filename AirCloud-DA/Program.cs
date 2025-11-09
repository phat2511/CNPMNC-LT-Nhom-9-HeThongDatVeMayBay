using AirCloud_DA.Data;
using AirCloud_DA.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure QuestPDF license (Community)
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Add Entity Framework
builder.Services.AddDbContext<AirCloudDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction =>
        {
            sqlServerOptionsAction.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        }
    ));

// Add custom services
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IFlightSearchService, FlightSearchService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();
builder.Services.AddScoped<ISupportService, SupportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Debug middleware - ĐẶT TRƯỚC MapRazorPages
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Path}");
    await next();
});

app.MapRazorPages();

app.Run();