using System.Text.RegularExpressions;
using MediaHarbor.Bot.Domain.ContentProcessing;
using MediaHarbor.Bot.Events;
using SimpleKafka.Interfaces;
using TBot.Core.RequestOptions;
using TBot.Core.TBot;
using TBot.Core.UpdateEngine;

namespace MediaHarbor.Bot.UpdatePipelines;

public partial class ContentPipeline(IKafkaProducer<string> kafkaProducer, ITBotClient botClient) : UpdatePipeline
{
    [GeneratedRegex(@"https://\w*.tiktok.com/\S*", RegexOptions.Compiled)]
    private static partial Regex TikTokLinkRegex();
    
    public override async Task<Context> ExecuteAsync(Context context)
    {
        var update = context.Update;
        if (update.IsMessage() && TryGetContent(context, out var processEvent)) {
            await kafkaProducer.ProduceAsync(processEvent);
            await botClient.DeleteMessageAsync(new DeleteMessageOptions
            {
                ChatId = context.Session.ChatId,
                MessageId = update.Message!.MessageId
            });
        }

        return await ExecuteNextAsync(context);
    }
    
    private static bool TryGetContent(Context context, out StartContentProcessEvent? processEvent)
    {
         var tiktokLink = TikTokLinkRegex()
            .Matches(context.Update.Message!.Text!)
            .FirstOrDefault()?.Value;

         if (!string.IsNullOrEmpty(tiktokLink)) {
             processEvent = BuildEvent(context, ContentProvider.TikTok, tiktokLink);
             return true;
         }

         processEvent = null;
         return false;
    }

    private static StartContentProcessEvent BuildEvent(Context context, ContentProvider provider, string link)
    {
        return new StartContentProcessEvent
        {
            Id = Guid.NewGuid(),
            ContentProvider = provider,
            SessionId = context.Session.Id,
            ChatId = context.Session.ChatId,
            Link = link
        };
    }
}