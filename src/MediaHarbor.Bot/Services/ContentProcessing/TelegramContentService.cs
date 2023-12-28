using MediaHarbor.Bot.Domain.ContentProcessing;
using MediaHarbor.Bot.Domain.DataLake;
using TBot.Core.RequestOptions;
using TBot.Core.RequestOptions.Inputs.Media;
using TBot.Core.TBot;
using TBot.Core.TBot.RequestIdentification;

namespace MediaHarbor.Bot.Services.ContentProcessing;

public class TelegramContentService(ITBotClient botClient, IDataLake dataLake) : ITelegramContentService
{
    public async Task SendContentAsync(ProcessedContent processedContent)
    {
        if (processedContent.ContentType == ContentType.Video) {
            await SendVideoAsync(processedContent);
        }

        if (processedContent.ContentType == ContentType.Images)
        {
            await SendImageAsync(processedContent);
            if (!string.IsNullOrEmpty(processedContent.AudioPath)) {
                await SendAudioAsync(processedContent);
            }
        }
    }

    private async Task SendVideoAsync(ProcessedContent processedContent)
    {
        await botClient.SendVideoAsync(new SendVideoOptions
        {
            Video = new MemoryStream(await dataLake.GetAsync(processedContent.VideoPath!)),
            ChatId = CurrentSessionThread.Session!.ChatId
        });
    }

    private async Task SendImageAsync(ProcessedContent processedContent)
    {
        var mediaGroup = new SendMediaGroupOptions {
            ChatId = CurrentSessionThread.Session!.ChatId,
            MediaSet = []
        };
            
        var paths = processedContent.ImagesPaths!;
        for (var i = 0; i < paths.Count; i++)
        {
            mediaGroup.MediaSet.Add(new InputMediaPhoto
            {
                Media = new MediaAttach
                {
                    Name = $"{processedContent.Id}_{i}",
                    Value = new MemoryStream(await dataLake.GetAsync(paths[i]))
                }
            });
        }
        
        await botClient.SendMediaGroupAsync(mediaGroup);
    }

    private async Task SendAudioAsync(ProcessedContent processedContent)
    {
        var mediaGroup = new SendMediaGroupOptions {
            ChatId = CurrentSessionThread.Session!.ChatId,
            MediaSet =
            [
                new InputMediaAudio
                {
                    Media = new MediaAttach
                    {
                        Name = $"{processedContent.Id}_audio",
                        Value = new MemoryStream(await dataLake.GetAsync(processedContent.AudioPath!))
                    }
                }
            ]
        };

        await botClient.SendMediaGroupAsync(mediaGroup);
    }
}