using MediaHarbor.Bot.Domain.DataLake;

namespace MediaHarbor.Bot.Services.DataLake;

public class FileDataLake : IDataLake
{
    public bool CreateFolderIfNotExist(string path)
    {
        if (Directory.Exists(path)) return false;
        Directory.CreateDirectory(path);
        return true;
    }
    
    public Task SaveAsync(string path, byte[] bytes)
    {
        return File.WriteAllBytesAsync(path, bytes);
    }
}