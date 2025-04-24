using CodeLifeAcademy.Core.Entities;

namespace CodeLifeAcademy.Application.DTOs;

public class UserUpdateDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public List<Guid> RoleIds { get; set; }
}
