using CodeLifeAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeLifeAcademy.Infrastructure.Persistence.Configturations;

public class CourseConfiguration: IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);
        builder.
            HasMany(c => c.Topics)
            .WithOne(t => t.Course)
            .HasForeignKey(t => t.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
