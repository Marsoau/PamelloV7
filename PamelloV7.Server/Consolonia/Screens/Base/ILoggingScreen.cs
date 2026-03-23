using PamelloV7.Framework.Modules;

namespace PamelloV7.Server.Consolonia.Screens.Base;

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
