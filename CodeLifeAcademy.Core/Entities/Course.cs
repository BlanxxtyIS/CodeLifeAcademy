namespace CodeLifeAcademy.Core.Entities;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
