using MediaHarbor.Bot.Domain.ContentProcessing;
using MediaHarbor.Bot.Events;
using SimpleKafka.Interfaces;

namespace MediaHarbor.Bot.Handlers;

public class ContentHandler(IContentService contentService) : IKafkaHandler<StartContentProcessEvent>
{
    public Task HandleAsync(StartContentProcessEvent processEvent)
    {
        return contentService.ProcessAsync(new Content
        {
            ContentProvider = processEvent.ContentProvider,
            Link = processEvent.Link
        });
    }
}