using MediaHarbor.Bot.Domain.ContentProcessing;
using SimpleKafka.Modules;

namespace MediaHarbor.Bot.Events;

public class StartContentProcessEvent : BaseEvent
{
    public Guid SessionId { get; set; }
    public long ChatId { get; set; }
    public ContentProvider ContentProvider { get; set; }
    public string Link { get; set; } = null!;
}