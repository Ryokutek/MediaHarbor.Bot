namespace MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;

public class TikTokAudio
{
    public string? DownloadLink { get; set; }
    public int Duration { get; set; }
    public byte[] Bytes { get; set; } = Array.Empty<byte>();
}