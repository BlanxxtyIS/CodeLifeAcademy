using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Application.Interfaces;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Infrastructure.Persistence;
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
    public readonly ApplicationDbContext _context;
    public readonly IPasswordHasher<User> _passwordHasher;
    public readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context,
                        IPasswordHasher<User> passwordHasher,
                        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<Guid?> RegisterAsync(RegisterUserDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return null;
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email
        };

        var hashedPassword = _passwordHasher
            .HashPassword(user, request.Password);
        user.PasswordHash = hashedPassword;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user.Id;
    }

    public async Task<AuthResultDto?> LoginAsync(LoginUserDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Username == request.Username);

        if (user is null)
        {
            return null;
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return await CreateTokenResponse(user);
    }

    private async Task<AuthResultDto> CreateTokenResponse(User? user)
    {
        return new AuthResultDto
        {
            AccesToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user),
            ExpiresAt = DateTime.UtcNow
        };
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
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
            Token = GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            User = user
        };

        _context.RefreshTokens.Add(refreshToken);
        user.RefreshToken = refreshToken;
        await _context.SaveChangesAsync();

        return refreshToken.Token;
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public async Task<AuthResultDto?> RefreshToken(RefreshTokenDto request)
    {
        var user = await ValidateRefreshToken(request.UserId, request.RefreshToken);

        if (user is null)
        {
            return null;
        }

        return await CreateTokenResponse(user);
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
