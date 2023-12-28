namespace MediaHarbor.Bot.Abstracts.ChainResponsibility;

public class AbstractHandler<T> : IHandler<T>
{
    private IHandler<T>? _nextHandler;
    
    public IHandler<T> SetNext(IHandler<T> handler)
    { 
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<T?> HandleAsync(T context)
    {
        if (_nextHandler is not null) {
            return await _nextHandler.HandleAsync(context);
        }
        
        return context;
    }
}