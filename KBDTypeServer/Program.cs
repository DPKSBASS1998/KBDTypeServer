using KBDTypeServer.Application;
using KBDTypeServer.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using KBDTypeServer.WebApi.Mapping;
using System.Text.Json;
using KBDTypeServer.Infrastructure.Repositories.OrderRepositories;
using KBDTypeServer.Domain.Entities.UserEntity;


var builder = WebApplication.CreateBuilder(args);

// ��������� ��������� ���� �����
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ���� ������
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }); ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/// <summary>
/// <!-- ��������� ������ -->-->
/// </summary>

/// <summary>
/// <!-- ��������� ���������� -->-->
/// </summary>
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

/// <summary>
/// <!-- ��������� AutoMapper -->-->
/// </summary>
builder.Services.AddAutoMapper(typeof(MappingProfile)); // ���� ���� � WebApi

/// <summary>
/// ������������ Identity
/// </summary>
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 0;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

/// <summary>
/// ������������ CORS
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

/// <summary>
/// ������������ ��� ��� ��������������
/// </summary>
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.SlidingExpiration = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsProduction())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");
}



app.Run();