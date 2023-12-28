using SimpleKafka.Modules;

namespace MediaHarbor.Bot.Events;

public class FinishContentProcessEvent : BaseEvent
{
    public Guid SessionId { get; set; }
    public long ChatId { get; set; }
    public Guid ContentId { get; set; }
    public int ContentType { get; set; }
    public string? AudioPath { get; set; }
    public string? VideoPath { get; set; }
    public List<string>? ImagesPaths { get; set; }
}