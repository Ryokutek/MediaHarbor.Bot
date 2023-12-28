using MediaHarbor.Bot.Domain.ContentProcessing;
using MediaHarbor.Bot.Events;
using SimpleKafka.Interfaces;
using TBot.Core.TBot.RequestIdentification;

namespace MediaHarbor.Bot.Handlers;

public class FinishContentHandler(ITelegramContentService contentService) : IKafkaHandler<FinishContentProcessEvent>
{
    public Task HandleAsync(FinishContentProcessEvent finishContentEvent)
    {
        using (CurrentSessionThread.SetSession(Session.Create(finishContentEvent.SessionId, finishContentEvent.ChatId)))
        {
            return contentService.SendContentAsync(new ProcessedContent
            {
                Id = finishContentEvent.ContentId,
                AudioPath = finishContentEvent.AudioPath,
                VideoPath = finishContentEvent.VideoPath,
                ImagesPaths = finishContentEvent.ImagesPaths,
                ContentType = (ContentType)finishContentEvent.ContentType
            });
        }
    }
}