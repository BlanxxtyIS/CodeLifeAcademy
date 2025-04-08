using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CodeLifeAcademy.Core.Entities;

namespace CodeLifeAcademy.Infrastructure.Persistence.Configturations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.Username).IsRequired();

        builder.HasOne(x => x.RefreshToken)
               .WithOne(x => x.User)
               .HasForeignKey<RefreshToken>(x => x.UserId)
               .IsRequired();

        builder.HasMany(x => x.UserRoles)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId);
    }
}
