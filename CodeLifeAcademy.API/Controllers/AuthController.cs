using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Application.Interfaces;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeLifeAcademy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _context;

    public AuthController(IAuthService authService,
                          ApplicationDbContext context)
    {
        _authService = authService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterUserDto request)
    {
        var userId = await _authService.RegisterAsync(request);

        if (userId is null)
        {
            return BadRequest("Ошибка регистрации");
        }
        return Ok(userId);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginUserDto request)
    {
        var result = await _authService.LoginAsync(request, Response);
        if (result is null || result.AccesToken is null)
        {
            return BadRequest("Неправильный логин или пароль");
        }

        return Ok(result.AccesToken);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResultDto>> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Refresh Token отсутствует");
        }

        var user = await _context.Users
            .Include(u => u.RefreshToken)
            .FirstOrDefaultAsync(u => u.RefreshToken.Token == refreshToken);

        if (user == null || user.RefreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized("Неверный токен или срок истек");
        }

        var result = await _authService.CreateTokenResponse(user, Response);

        return Ok(result);
    }
}
