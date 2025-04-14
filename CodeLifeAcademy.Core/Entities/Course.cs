namespace CodeLifeAcademy.Core.Entities;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int TimeInMinutes { get; set; }
    public string TimeFormatted => $"{TimeInMinutes / 60}ч {TimeInMinutes % 60}м";

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
