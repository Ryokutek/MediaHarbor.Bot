namespace MediaHarbor.Bot.Abstracts.ChainResponsibility;

public interface IHandler<T>
{
    IHandler<T> SetNext(IHandler<T> handler);
    Task<T?> HandleAsync(T context);
}