using NetCord.Logging;
using PamelloV7.Framework.Logging;

namespace PamelloV7.Module.Marsoau.NetCord.Logger;

public class NetCordLogger : IGatewayLogger
{
    public void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        if (!IsEnabled(logLevel)) return;
        
        Output.Write($"Logger: {formatter(state, exception)}");
    }
    
    public bool IsEnabled(LogLevel logLevel) {
        return logLevel > LogLevel.Information;
    }
}
