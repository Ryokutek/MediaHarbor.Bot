namespace MediaHarbor.Bot.Options;

public class RedisOptions
{
    public string? Host { get; set; }
    public string? Password { get; set; }
    public int DefaultDatabase { get; set; }
    public long SyncTimeout { get; set; }
}