namespace MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;

public class ProcessTikTokContent
{
    public Guid Id { get; init; }
    public string OriginalLink { get; set; } = null!;
    public string? AwemeId { get; set; }
    public string? Username { get; set; }
    public TikTokVideo? TikTokVideo { get; set; }
    public TikTokAudio? TikTokAudio { get; set; }
    public List<TikTokImage>? TikTokImages { get; set; }

    public bool IsImages() => TikTokImages is not null;
    public bool IsAudioExist() => TikTokAudio is not null;
    public bool IsVideo() => TikTokVideo is not null;
}