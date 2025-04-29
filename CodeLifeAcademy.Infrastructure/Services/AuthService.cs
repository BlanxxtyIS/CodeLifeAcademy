using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Application.Interfaces;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Core.Enums;
using CodeLifeAcademy.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CodeLifeAcademy.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IJwtService _jwtService;

    public AuthService(ApplicationDbContext context,
                        IPasswordHasher<User> passwordHasher,
                        IConfiguration configuration,
                        IJwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _jwtService = jwtService;
    }

    public async Task<Guid?> RegisterAsync(RegisterUserDto request)
    {
        // Проверяем уникальность email и username.
        if (await _context.Users
            .AnyAsync(u => u.Username == request.Username || 
            u.Email == request.Email))
        {
            return null;
        }

        // Создается новый юзер с ролью Student
        var user = new User
        {
            Username = request.Username,
            Email = request.Email 
        };

        var studentRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == UserRoleEnum.Student.ToString());

        if (studentRole == null)
        {
            return null;
        }

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = studentRole.Id
        };
        
        //Хешируем пасс и сохраеняем в бд
        var hashedPassword = _passwordHasher
            .HashPassword(user, request.Password);
        user.PasswordHash = hashedPassword;

        _context.Users.Add(user);
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return user.Id;
    }
  
    public async Task<AuthResultDto?> LoginAsync(LoginUserDto request, HttpResponse response)
    {
        //Ищем юзера 
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null)
        {
            return null;
        }

        //Проверяем пасс
        if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
        {
            return null;
        }

        //Если успешно создаем токены и возвращаем.
        return await CreateTokenResponse(user, response);
    }

    public async Task<AuthResultDto> CreateTokenResponse(User user, HttpResponse response)
    {
        var refreshToken = await GenerateAndSaveRefreshTokenAsync(user);

        response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return new AuthResultDto
        {
            AccesToken = CreateToken(user, response),
            ExpiresAt = DateTime.UtcNow
        };
    }

    private string CreateToken(User user, HttpResponse response)
    {
        var roles = user.UserRoles.Select(ur => ur.Role.Name);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = _jwtService.CreateAccessToken(claims);

        response.Cookies.Append("accessToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)

        });

        return token;
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.UserId == user.Id);

        if (existingToken != null)
        {
            _context.RefreshTokens.Remove(existingToken);
        }
        var refreshToken = new RefreshToken()
        {
            Token = _jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            User = user
        };

        _context.RefreshTokens.Add(refreshToken);
        user.RefreshToken = refreshToken;
        await _context.SaveChangesAsync();

        return refreshToken.Token;
    }

    private async Task<User?> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        var user = await _context.Users
            .Include(u => u.RefreshToken)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null || user.RefreshToken.Token != refreshToken || user.RefreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }
        return user;
    }
}
