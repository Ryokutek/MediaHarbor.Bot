using System.Text.RegularExpressions;
using MediaHarbor.Bot.Abstracts.ChainResponsibility;
using MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;

namespace MediaHarbor.Bot.Services.ContentProcessing.TikTok.Handlers;

public partial class RefererLinkHandler(HttpClient httpClient) : AbstractHandler<ProcessTikTokContent>
{
    public override async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent content)
    {
        if (content.OriginalLink.Contains("https://www.tiktok.com/@"))
        {
            content.OriginalLink = BuildOriginalLink(new Uri(content.OriginalLink));
            return await base.HandleAsync(content);
        }

        var refererLink = await GetRefererLinkAsync(content.OriginalLink);
        if (string.IsNullOrEmpty(refererLink)) {
            return content;
        }

        content.OriginalLink = refererLink;
        return await base.HandleAsync(content);
    }
    
    private async Task<string?> GetRefererLinkAsync(string link)
    {
        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, link));
        var uri = response.RequestMessage!.RequestUri;
        return uri is null ? null : BuildOriginalLink(uri);
    }

    private static string BuildOriginalLink(Uri uri)
    {
        return $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
    }

    
}