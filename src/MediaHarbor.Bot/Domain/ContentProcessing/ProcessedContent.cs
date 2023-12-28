namespace MediaHarbor.Bot.Domain.ContentProcessing;

public class ProcessedContent
{
    public Guid Id { get; set; }
    public ContentType ContentType { get; set; }
    public string? AudioPath { get; set; }
    public string? VideoPath { get; set; }
    public List<string>? ImagesPaths { get; set; }
}