namespace CodeLifeAcademy.Application.DTOs;

public class CreateLessionDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid TopicId { get; set; }
}
