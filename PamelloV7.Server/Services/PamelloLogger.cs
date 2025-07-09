using PamelloV7.Core.Services;

namespace PamelloV7.Server.Services;

public class PamelloLogger : IPamelloLogger
{
    public void Log(object? obj) {
        StaticLogger.Log(obj);
    }

    public void Warning(object? obj) {
        StaticLogger.Warning(obj);
    }

    public void Error(object? obj) {
        StaticLogger.Error(obj);
    }
}
