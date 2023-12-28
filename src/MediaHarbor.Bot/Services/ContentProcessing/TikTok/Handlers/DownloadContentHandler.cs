using MediaHarbor.Bot.Abstracts.ChainResponsibility;
using MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;

namespace MediaHarbor.Bot.Services.ContentProcessing.TikTok.Handlers;

public class DownloadContentHandler(HttpClient httpClient) : AbstractHandler<ProcessTikTokContent>
{
    public override async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent context)
    {
        if (context.IsVideo()) {
            context.TikTokVideo!.Bytes = await DownloadAsync(context.OriginalLink, context.TikTokVideo.DownloadLink!);
        }
        
        if (context.IsImages()) {
            foreach (var image in context.TikTokImages!)
            {
                image.Bytes = await DownloadAsync(context.OriginalLink, image.DownloadLink!);
            }
        }

        if (context.IsAudioExist()) {
            context.TikTokAudio!.Bytes = await DownloadAsync(context.OriginalLink, context.TikTokAudio.DownloadLink!);
        }
        
        return await base.HandleAsync(context);
    }

    private async Task<byte[]> DownloadAsync(string refererLink, string downloadLink)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, downloadLink);
        request.Headers.TryAddWithoutValidation(TikTokConstants.HeaderUserAgent, TikTokConstants.HeaderUserAgentValue);
        request.Headers.TryAddWithoutValidation(TikTokConstants.HeaderReferer, refererLink);
        
        var response = await httpClient.SendAsync(request);
        return await response.Content.ReadAsByteArrayAsync();
    }
}