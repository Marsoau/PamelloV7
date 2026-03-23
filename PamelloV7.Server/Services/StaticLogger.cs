using PamelloV7.Framework.Modules;
using PamelloV7.Server.Consolonia.Screens.Base;

namespace PamelloV7.Server.Services;

public static class StaticLogger
{
    public static ILoggingScreen Screen { get; set; } = null!;
    
    private static void BaseLog(object? obj, IPamelloModule? module, ELogLevel level) {
        Screen.WriteLine(obj, module, level);
        //Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [ {header} ] {obj}");
    }
    public static void Log(object? obj, IPamelloModule? module) {
        BaseLog(obj, module, ELogLevel.Log);
    }
    public static void Warning(object? obj, IPamelloModule? module) {
        BaseLog(obj, module, ELogLevel.Warning);
    }
    public static void Error(object? obj, IPamelloModule? module) {
        BaseLog(obj, module, ELogLevel.Error);
    }
}
