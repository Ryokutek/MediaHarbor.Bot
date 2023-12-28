namespace MediaHarbor.Bot.Domain.ContentProcessing;

public interface IContentService
{
    Task ProcessAsync(Content content);
}