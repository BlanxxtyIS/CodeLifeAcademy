using CodeLifeAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeLifeAcademy.Infrastructure.Persistence.Configturations;

public class LessionConfiguration: IEntityTypeConfiguration<Lession>
{ 
    public void Configure(EntityTypeBuilder<Lession> builder)
    {
        builder.HasKey(l => l.Id);

        builder.HasOne(l => l.Topic)
            .WithMany(t => t.Lessions)
            .HasForeignKey(l => l.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
