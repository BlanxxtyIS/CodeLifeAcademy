using CodeLifeAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace CodeLifeAcademy.Infrastructure.Persistence.Configturations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Token).IsUnique();

        builder.HasOne(x => x.User)
               .WithOne(x => x.RefreshToken)
               .HasForeignKey<RefreshToken>(x => x.UserId);
    }
}