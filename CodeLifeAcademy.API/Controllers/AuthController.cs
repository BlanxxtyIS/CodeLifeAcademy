using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Application.Interfaces;
using CodeLifeAcademy.Core.Entities;
using Microsoft.AspNetCore.Authorization;
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
        var result = await _authService.LoginAsync(request, Response);
        if (result is null || result.AccesToken is null || result.RefreshToken is null)
        {
            return BadRequest("Неправильный логин или пароль");
        }

        return Ok(result.AccesToken);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("test")] 
    public async Task<ActionResult<string>> Test()
    {
        return Ok("Only admins");
    }

    [Authorize(Policy = "MentorOrAdmin")]
    [HttpGet("test2")]
    public async Task<ActionResult<string>> Test2()
    {
        return Ok("Menthor and admin");
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResultDto>> Refresh(RefreshTokenDto request)
    {
        var refresh = await _authService.RefreshToken(request, Response);

        if (refresh is null || refresh.AccesToken is null || refresh.RefreshToken is null)
        {
            return Unauthorized("Рефреш токен не действителен");
        }
        return Ok(refresh);
    }
}
