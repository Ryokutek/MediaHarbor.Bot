using MediaHarbor.Bot.Abstracts.ChainResponsibility;
using MediaHarbor.Bot.Domain.ContentProcessing;
using MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;
using MediaHarbor.Bot.Services.ContentProcessing.TikTok.Handlers;

namespace MediaHarbor.Bot.Services.ContentProcessing.TikTok;

public class TikTokContentService(IServiceProvider serviceProvider) : IContentProviderService
{
    public ContentProvider ContentProvider => ContentProvider.TikTok;

    public async Task ProcessAsync(Content content)
    {
        AbstractHandler<ProcessTikTokContent> handler = ActivatorUtilities.CreateInstance<RefererLinkHandler>(serviceProvider);
        
        handler
            .SetNext(ActivatorUtilities.CreateInstance<ContentInformationHandler>(serviceProvider))
            .SetNext(ActivatorUtilities.CreateInstance<DownloadContentHandler>(serviceProvider))
            .SetNext(ActivatorUtilities.CreateInstance<SaveContentHandler>(serviceProvider));
        
        await handler.HandleAsync(new ProcessTikTokContent { Id = Guid.NewGuid(), OriginalLink = content.Link });
    }
}