using MediaHarbor.Bot.Domain.ContentProcessing;
using MediaHarbor.Bot.Events;
using SimpleKafka.Interfaces;
using TBot.Core.TBot.RequestIdentification;

namespace MediaHarbor.Bot.Handlers;

public class ContentHandler(IContentService contentService) : IKafkaHandler<StartContentProcessEvent>
{
    public Task HandleAsync(StartContentProcessEvent processEvent)
    {
        using (CurrentSessionThread.SetSession(Session.Create(processEvent.SessionId, processEvent.ChatId)))
        {
            return contentService.ProcessAsync(new Content
            {
                Id = Guid.NewGuid(),
                ContentProvider = processEvent.ContentProvider,
                Link = processEvent.Link
            });
        }
    }
}