using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Core.Entities;

namespace CodeLifeAcademy.Application.Interfaces;

public interface IAuthService
{
    Task<Guid?> RegisterAsync(RegisterUserDto request);
    Task<AuthResultDto?> LoginAsync(LoginUserDto request);
    Task<AuthResultDto?> RefreshToken(RefreshTokenDto request);
}
