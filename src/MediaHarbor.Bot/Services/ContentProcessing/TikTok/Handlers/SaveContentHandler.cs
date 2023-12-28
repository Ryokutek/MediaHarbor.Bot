using MediaHarbor.Bot.Abstracts.ChainResponsibility;
using MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;
using MediaHarbor.Bot.Domain.DataLake;

namespace MediaHarbor.Bot.Services.ContentProcessing.TikTok.Handlers;

public class SaveContentHandler(IDataLake dataLake) : AbstractHandler<ProcessTikTokContent>
{
    private static string GetContentFolder() => Path.Combine(Constants.ContentFolder, TikTokConstants.ContentFolder);
    private static string BuildVideoPath(string awemeId) => Path.Combine(GetContentFolder(), $"{awemeId}.mp4");
    private static string GetImagesFolder(string awemeId) => Path.Combine(GetContentFolder(), awemeId);
    private static string BuildImagesPath(string awemeId, int count) => Path.Combine(GetImagesFolder(awemeId), $"{count}.png");
    
    public override async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent context)
    {
        dataLake.CreateFolderIfNotExist(GetContentFolder());
        
        if (context.IsVideo()) {
            await dataLake.SaveAsync(BuildVideoPath(context.AwemeId!), context.TikTokVideo!.Bytes);
        }

        if (context.IsImages())
        {
            dataLake.CreateFolderIfNotExist(GetImagesFolder(context.AwemeId!));
            for (var i = 0; i < context.TikTokImages!.Count; i++)
            {
                await dataLake.SaveAsync(BuildImagesPath(context.AwemeId!, i), context.TikTokImages[i].Bytes);
            }
        }
        
        return await base.HandleAsync(context);
    }
}