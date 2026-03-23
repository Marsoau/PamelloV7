using System.Reflection;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Logging;

public static class StaticLogger
{
    public static ILoggingScreen LoadingScreen { get; set; } = null!;
    public static ILoggingScreen? MainScreen { get; set; } = null;
    
    public static IAssemblyTypeResolver? Types { get; set; } = null;
    
    private static ILoggingScreen Screen => MainScreen ?? LoadingScreen;
    
    private static void BaseLog(object? obj, Assembly assembly, ELogLevel level) {
        Screen.WriteLine(obj, Types?.GetAssemblyModule(assembly), level);
        //StaticLogger.Log($"{DateTime.Now:HH:mm:ss.fff} [ {header} ] {obj}");
    }
    public static void Log(object? obj = null) {
        var assembly = Assembly.GetCallingAssembly();
        BaseLog(obj, assembly, ELogLevel.Log);
    }
    public static void Warning(object? obj = null) {
        var assembly = Assembly.GetCallingAssembly();
        BaseLog(obj, assembly, ELogLevel.Warning);
    }
    public static void Error(object? obj = null) {
        var assembly = Assembly.GetCallingAssembly();
        BaseLog(obj, assembly, ELogLevel.Error);
    }
}
