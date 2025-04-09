using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Core.Enums;
using Microsoft.EntityFrameworkCore;
namespace CodeLifeAcademy.Infrastructure.Persistence.Configturations;

public class RoleSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var roles = new List<Role>
    {
        new Role {
            Id = Guid.Parse("11111111-2222-1111-1111-111111111111"), Name = UserRoleEnum.Admin.ToString() 
        },
        new Role {
            Id = Guid.Parse("22222222-3333-2222-2222-222222222222"), Name = UserRoleEnum.Mentor.ToString()
        },
        new Role {
            Id = Guid.Parse("33333333-4444-3333-3333-333333333333"), Name = UserRoleEnum.Student.ToString() 
        }
    };

        modelBuilder.Entity<Role>().HasData(roles);
    }
}
