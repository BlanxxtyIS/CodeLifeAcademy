using System.ComponentModel.DataAnnotations;

namespace CodeLifeAcademy.Core.Entities;

public class Lession
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid TopicId { get; set; }
    public Topic Topic { get; set; } = null!;

    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
}
