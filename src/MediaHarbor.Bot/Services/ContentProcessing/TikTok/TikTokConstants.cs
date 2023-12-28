namespace MediaHarbor.Bot.Services.ContentProcessing.TikTok;

public static class TikTokConstants
{
    public const string HeaderUserAgentValue = "TikTok 26.2.0 rv:262018 (iPhone; iOS 14.4.2; en_US) Cronet";
    public const string HeaderUserAgent = "User-Agent";
    public const string HeaderReferer = "Referer";
    public const string ContentFolder = "tiktok";
    
    public static string GetContentFolder() => Path.Combine(Constants.ContentFolder, ContentFolder);
    public static string BuildVideoPath(Guid contentId) => Path.Combine(GetContentFolder(), $"{contentId}.mp4");
    public static string GetImagesFolder(Guid contentId) => Path.Combine(GetContentFolder(), contentId.ToString());
    public static string BuildImagesPath(Guid contentId, int count) => Path.Combine(GetImagesFolder(contentId), $"{count}.png");
    public static string BuildAudioPath(Guid contentId) => Path.Combine(GetImagesFolder(contentId), "audio.mp3");
}