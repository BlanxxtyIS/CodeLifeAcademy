using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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
        var users = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync();
        return Ok(users);
    }

    [HttpGet]
    [Route("roles")]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        var roles = await _context.Roles.ToListAsync();
        return Ok(roles);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto dto)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();

        // Обновляем базовые поля
        user.Username = dto.Username;
        user.Email = dto.Email;

        // Обновляем роли
        var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();
        var newRoleIds = dto.RoleIds.ToHashSet();

        // Удаляем невыбранные роли
        foreach (var userRole in user.UserRoles.ToList())
        {
            if (!newRoleIds.Contains(userRole.RoleId))
            {
                _context.UserRoles.Remove(userRole);
            }
        }

        // Добавляем новые роли
        foreach (var roleId in newRoleIds)
        {
            if (!existingRoleIds.Contains(roleId))
            {
                user.UserRoles.Add(new UserRole { RoleId = roleId });
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpGet("userinfo")]
    public IActionResult GetUserInfo()
    {
        var claims = User.Claims.Select(c => new
        {
            Type = c.Type,
            Value = c.Value
        });

        return Ok(new
        {
            Claims = claims
        });
    }
}
