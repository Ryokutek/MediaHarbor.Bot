using MediaHarbor.Bot.Domain.ContentProcessing;
namespace MediaHarbor.Bot.Services.ContentProcessing;

public class ContentService(IServiceProvider serviceProvider) : IContentService
{
    public Task ProcessAsync(Content content)
    {
        var service = serviceProvider.GetRequiredKeyedService<IContentProviderService>(Enum.GetName(content.ContentProvider));
        return service.ProcessAsync(content);
    }
}