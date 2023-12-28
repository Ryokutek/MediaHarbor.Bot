using MediaHarbor.Bot.Abstracts.ChainResponsibility;
using MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;
using MediaHarbor.Bot.Domain.DataLake;

namespace MediaHarbor.Bot.Services.ContentProcessing.TikTok.Handlers;

public class SaveContentHandler(IDataLake dataLake) : AbstractHandler<ProcessTikTokContent>
{
    public override async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent content)
    {
        dataLake.CreateFolderIfNotExist(TikTokConstants.GetContentFolder());
        
        if (content.IsVideo()) {
            await dataLake.SaveAsync(TikTokConstants.BuildVideoPath(content.Id), content.TikTokVideo!.Bytes);
        }

        if (content.IsImages())
        {
            dataLake.CreateFolderIfNotExist(TikTokConstants.GetImagesFolder(content.Id));
            for (var i = 0; i < content.TikTokImages!.Count; i++)
            {
                await dataLake.SaveAsync(TikTokConstants.BuildImagesPath(content.Id, i), content.TikTokImages[i].Bytes);
            }
        }

        if (content.IsAudioExist()) {
            await dataLake.SaveAsync(TikTokConstants.BuildAudioPath(content.Id), content.TikTokAudio!.Bytes);
        }
        
        return await base.HandleAsync(content);
    }
}