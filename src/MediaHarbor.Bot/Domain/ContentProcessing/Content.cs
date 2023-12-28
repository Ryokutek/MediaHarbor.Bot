namespace MediaHarbor.Bot.Domain.ContentProcessing;

public class Content
{
    public Guid Id { get; set; }
    public ContentProvider ContentProvider { get; set; }
    public string Link { get; set; } = null!;
}