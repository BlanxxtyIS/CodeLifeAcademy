using CodeLifeAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeLifeAcademy.Infrastructure.Persistence.Configturations;

public class TopicConfiguration: IEntityTypeConfiguration<Topic> 
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Course)
            .WithMany(c => c.Topics)
            .HasForeignKey(t => t.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Lessions)
            .WithOne(l => l.Topic)
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
