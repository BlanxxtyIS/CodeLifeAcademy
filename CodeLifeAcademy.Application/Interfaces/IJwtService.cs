using System.Security.Claims;

namespace CodeLifeAcademy.Application.Interfaces;

public interface IJwtService
{
    string CreateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}
