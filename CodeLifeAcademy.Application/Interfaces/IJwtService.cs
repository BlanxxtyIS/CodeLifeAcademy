using System.Security.Claims;

namespace CodeLifeAcademy.Application.Interfaces;

public interface IJwtService
{
    ClaimsPrincipal? ValidateToken(string token);
}
