namespace CodeLifeAcademy.Core.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public RefreshToken? RefreshToken { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
