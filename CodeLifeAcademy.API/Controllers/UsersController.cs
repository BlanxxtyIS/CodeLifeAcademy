using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeLifeAcademy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IValidator<CreateUserDto> _createUserValidator;

    public UsersController(ApplicationDbContext context, IValidator<CreateUserDto> createUserValidator)
    {
        _context = context;
        _createUserValidator = createUserValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }
}
