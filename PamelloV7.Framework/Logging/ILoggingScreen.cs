using PamelloV7.Framework.Modules;

namespace PamelloV7.Framework.Logging;

public enum ELogLevel
{
    Log,
    Warning,
    Error,
}

public interface ILoggingScreen
{
    public void WriteLine(object? obj, IPamelloModule? module, ELogLevel level);
}
