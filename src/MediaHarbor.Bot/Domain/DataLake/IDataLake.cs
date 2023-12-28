namespace MediaHarbor.Bot.Domain.DataLake;

public interface IDataLake
{
    bool CreateFolderIfNotExist(string path);
    Task SaveAsync(string path, byte[] bytes);
}