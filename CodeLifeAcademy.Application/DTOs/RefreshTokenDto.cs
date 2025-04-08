using CodeLifeAcademy.Core.Entities;

namespace CodeLifeAcademy.Application.DTOs;

public class RefreshTokenDto
{
    public Guid UserId { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
