using System.Text.RegularExpressions;
using MediaHarbor.Bot.Abstracts.ChainResponsibility;
using MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;
using Newtonsoft.Json.Linq;

namespace MediaHarbor.Bot.Services.ContentProcessing.TikTok.Handlers;

public partial class ContentInformationHandler(HttpClient httpClient) : AbstractHandler<ProcessTikTokContent>
{
    private const string ContentInformationRequestUrl = "https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/feed/";
    
    [GeneratedRegex(@"\/video\/(\d+)", RegexOptions.Compiled)]
    private static partial Regex GetAwemeId();
    
    [GeneratedRegex(@"\/(@[a-zA-Z0-9_]+)", RegexOptions.Compiled)]
    private static partial Regex GetUsername();
    
    public override async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent content)
    {
        content.AwemeId = GetAwemeId().Match(content.OriginalLink).Groups[1].Value;
        content.Username = GetUsername().Match(content.OriginalLink).Groups[1].Value;
        
        var contentInformation = await GetContentInformationAsync(content);
        if (contentInformation["aweme_list"]?[0]?["aweme_id"]?.ToString() != content.AwemeId) {
            return content;
        }

        if (contentInformation["aweme_list"]?[0]?["image_post_info"] is null) {
            content.TikTokVideo = ExtractVideoInformation(contentInformation);
        }
        else {
            content.TikTokImages = ExtractTikTokImages(contentInformation).ToList();
        }
        
        return await base.HandleAsync(content);
    }

    private async Task<JToken> GetContentInformationAsync(ProcessTikTokContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{ContentInformationRequestUrl}?aweme_id={content.AwemeId}");
        request.Headers.TryAddWithoutValidation(TikTokConstants.HeaderUserAgent, TikTokConstants.HeaderUserAgentValue);
        
        var response = await httpClient.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        
        return JToken.Parse(responseString);
    }

    private static TikTokVideo ExtractVideoInformation(JToken contentInformation)
    {
        return new TikTokVideo
        {
            DownloadLink = contentInformation["aweme_list"]?[0]?["video"]?["play_addr"]?["url_list"]?[0]?.ToString(),
            Duration = int.Parse(contentInformation["aweme_list"]?[0]?["video"]?["duration"]?.ToString() ?? "0"),
            Height = int.Parse(contentInformation["aweme_list"]?[0]?["video"]?["play_addr"]?["height"]?.ToString() ?? "0"),
            Width = int.Parse(contentInformation["aweme_list"]?[0]?["video"]?["play_addr"]?["width"]?.ToString() ?? "0")
        };
    }

    private static IEnumerable<TikTokImage> ExtractTikTokImages(JToken contentInformation)
    {
        var images = contentInformation["aweme_list"][0]?["image_post_info"]?["images"];
        foreach (var image in images!)
        {
            yield return new TikTokImage
            {
                Height = int.Parse(image["display_image"]?["height"]?.ToString() ?? "0"),
                Width = int.Parse(image["display_image"]?["width"]?.ToString() ?? "0"),
                DownloadLink = image["display_image"]?["url_list"]?[0]?.ToString()
            };
        }
    }
}