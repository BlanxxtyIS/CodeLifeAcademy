using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace CodeLifeAcademy.Application.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Регистрирует нового пользователя, возвращает его id или null если ошибка.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Guid?> RegisterAsync(RegisterUserDto request);

    /// <summary>
    /// Аутентифицирует пользователя и возвращает результат с токенами 
    /// (Access, refresh token и дату действия до)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    Task<AuthResultDto?> LoginAsync(LoginUserDto request, HttpResponse response);

    /// <summary>
    /// Обновляет Access token используя refresh token и возвращет результат с токенами
    /// (Access, refresh token и дату действия до)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    Task<AuthResultDto?> RefreshToken(RefreshTokenDto request, HttpResponse response);
}
