namespace CodeLifeAcademy.Application.DTOs;

public class CreateLessionDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid TopicId { get; set; }
}
