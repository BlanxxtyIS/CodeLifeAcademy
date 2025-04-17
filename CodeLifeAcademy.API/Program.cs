using CodeLifeAcademy.Application.Interfaces;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Infrastructure.DependencyInjection;
using CodeLifeAcademy.Infrastructure.Services;
using CodeLifeAcademy.API.Middleware;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CodeLifeAcademy.Application.Validators;
using FluentValidation;
using Serilog;

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
//    .Enrich.FromLogContext()
//    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog();

builder.Logging.AddConsole();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSingleton<IJwtService, JwtService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
    };

    // Мы НЕ читаем токен из заголовка, т.к. читаем его из cookie вручную в JwtCookieMiddleware
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = null;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("MentorOrAdmin", policy => policy.RequireRole("Admin", "Mentor"));
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateCourseValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder
            .WithOrigins("https://localhost:5002") // адрес клиента
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // 👈 обязательно!
    });
});

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<JwtCookieMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
