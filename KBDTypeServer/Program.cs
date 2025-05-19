using System.IO;
using KBDTypeServer.Models.Data;
using KBDTypeServer.Models.Users;
using KBDTypeServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ��������� ��������� ���� �����
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ���� ������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISwitchService, SwitchService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ������������ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.SlidingExpiration = true;
});

var app = builder.Build();

// ������� ��������� ����� � wwwroot
app.UseStaticFiles();

// ϳ��������� �������� ����� "assets" �� ��������
// ������������� ���������� ���� �� �����, �� ������� ����������� ����������
var assetFolderPath = @"C:\Users\bvivl\Documents\������������ ����������\my-app\src\assets";
if (Directory.Exists(assetFolderPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(assetFolderPath),
        RequestPath = "/assets"
    });
}
else
{
    // ������� ����������� ��������� ��� ������� �������
    Console.WriteLine($"Warning: Assets folder not found at {assetFolderPath}");
}

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

app.Run();