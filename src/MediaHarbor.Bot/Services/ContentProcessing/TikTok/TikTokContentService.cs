using MediaHarbor.Bot.Abstracts.ChainResponsibility;
using MediaHarbor.Bot.Domain.ContentProcessing;
using MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;
using MediaHarbor.Bot.Events;
using MediaHarbor.Bot.Services.ContentProcessing.TikTok.Handlers;
using SimpleKafka.Interfaces;
using TBot.Core.TBot.RequestIdentification;

namespace MediaHarbor.Bot.Services.ContentProcessing.TikTok;

public class TikTokContentService(IServiceProvider serviceProvider, IKafkaProducer<string> producer) : IContentProviderService
{
    public async Task ProcessAsync(Content content)
    {
        AbstractHandler<ProcessTikTokContent> handler = ActivatorUtilities.CreateInstance<RefererLinkHandler>(serviceProvider);
        
        handler
            .SetNext(ActivatorUtilities.CreateInstance<ContentInformationHandler>(serviceProvider))
            .SetNext(ActivatorUtilities.CreateInstance<DownloadContentHandler>(serviceProvider))
            .SetNext(ActivatorUtilities.CreateInstance<SaveContentHandler>(serviceProvider));
        
        var processedContent = await handler
            .HandleAsync(new ProcessTikTokContent { Id = content.Id, OriginalLink = content.Link });

        if (processedContent is null) {
            return;
        }
        
        await producer.ProduceAsync(new FinishContentProcessEvent
        {
            Id = Guid.NewGuid(),
            SessionId = CurrentSessionThread.Session!.Id,
            ChatId = CurrentSessionThread.Session.ChatId,
            AudioPath = processedContent.IsAudioExist() ? TikTokConstants.BuildAudioPath(content.Id) : default,
            VideoPath = processedContent.IsVideo() ? TikTokConstants.BuildVideoPath(content.Id) : default,
            ImagesPaths = processedContent.TikTokImages?.Select((_, index) => TikTokConstants.BuildImagesPath(content.Id, index)).ToList(),
            ContentId = content.Id,
            ContentType = processedContent.IsVideo() ? (int)ContentType.Video : (int)ContentType.Images
        }, string.Empty);
    }
}