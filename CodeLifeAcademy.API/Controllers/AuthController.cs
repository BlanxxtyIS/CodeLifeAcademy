using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Application.Interfaces;
using CodeLifeAcademy.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CodeLifeAcademy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterUserDto request)
    {
        var user = await _authService.RegisterAsync(request);
        if (user is null)
        {
            return BadRequest("Ошибка регистрации");
        }
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginUserDto request)
    {
        var result = await _authService.LoginAsync(request);
        if (result is null || result.AccesToken is null || result.RefreshToken is null)
        {
            return BadRequest("Неправильный логин или пароль");
        }

        return Ok(result.AccesToken);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResultDto>> Refresh(RefreshTokenDto request)
    {
        var refresh = await _authService.RefreshToken(request);

        if (refresh is null || refresh.AccesToken is null || refresh.RefreshToken is null)
        {
            return Unauthorized("Рефреш токен не действителен");
        }
        return Ok(refresh);
    }
}
