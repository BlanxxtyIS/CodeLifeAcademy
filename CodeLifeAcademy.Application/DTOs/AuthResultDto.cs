namespace CodeLifeAcademy.Application.DTOs;

public class AuthResultDto
{
    public string AccesToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
