namespace MediaHarbor.Bot.Domain.ContentProcessing;

public interface ITelegramContentService
{
    Task SendContentAsync(ProcessedContent processedContent);
}