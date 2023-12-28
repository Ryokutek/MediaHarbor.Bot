namespace MediaHarbor.Bot.Domain.ContentProcessing;

public interface IContentProviderService
{
    public ContentProvider ContentProvider { get; }
    Task ProcessAsync(Content content);
}