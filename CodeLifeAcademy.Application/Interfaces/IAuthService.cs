using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace CodeLifeAcademy.Application.Interfaces;

public interface IAuthService
{
    Task<Guid?> RegisterAsync(RegisterUserDto request);
    Task<AuthResultDto?> LoginAsync(LoginUserDto request, HttpResponse response);
    Task<AuthResultDto?> RefreshToken(RefreshTokenDto request, HttpResponse response);
}
