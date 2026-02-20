using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Services;

public interface IPamelloLogger : IPamelloService
{
    public void Log(object? obj);
    public void Warning(object? obj);
    public void Error(object? obj);
}
