using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Infrastructure.Persistence.Configturations;
using Microsoft.EntityFrameworkCore;

namespace CodeLifeAcademy.Infrastructure.Persistence;

public class ApplicationDbContext: DbContext 
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Lession> Lessions => Set<Lession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new TopicConfiguration());
        modelBuilder.ApplyConfiguration(new LessionConfiguration());

        RoleSeeder.Seed(modelBuilder);
    }
}
