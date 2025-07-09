namespace PamelloV7.Server.Services;

public static class StaticLogger
{
    private static void BaseLog(string header, object? obj) {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [ {header} ] {obj}");
    }
    public static void Log(object? obj) {
        BaseLog("Log", obj);
    }
    public static void Warning(object? obj) {
        BaseLog("Warning", obj);
    }
    public static void Error(object? obj) {
        BaseLog("Error", obj);
    }
}
