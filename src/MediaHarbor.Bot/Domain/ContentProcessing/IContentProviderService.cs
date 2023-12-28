namespace MediaHarbor.Bot.Domain.ContentProcessing;

public interface IContentProviderService
{
    Task ProcessAsync(Content content);
}