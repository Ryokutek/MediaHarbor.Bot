namespace MediaHarbor.Bot.Domain.ContentProcessing.Providers.TikTok;

public class TikTokImage
{
    public string? DownloadLink { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public byte[] Bytes { get; set; } = Array.Empty<byte>();
}