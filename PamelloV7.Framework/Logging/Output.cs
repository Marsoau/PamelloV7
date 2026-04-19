using System.Reflection;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging.Messages;
using PamelloV7.Framework.Logging.Services;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Logging;

public static class Output
{
    public static IPamelloLogger Logger { get; set; } = null!;

    public static LogMessage Write(object? obj = null, ELogLevel level = ELogLevel.Log) {
        var assembly = Assembly.GetCallingAssembly();

        return Logger.Write(obj, level, assembly);
    }
}
