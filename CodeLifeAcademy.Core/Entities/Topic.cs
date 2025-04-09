namespace CodeLifeAcademy.Core.Entities;

public class Topic
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<Lession> Lessions { get; set; } = new List<Lession>();
}
