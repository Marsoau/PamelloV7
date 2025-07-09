using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface IPamelloLogger : IPamelloService
{
    public void Log(object? obj);
    public void Warning(object? obj);
    public void Error(object? obj);
}
