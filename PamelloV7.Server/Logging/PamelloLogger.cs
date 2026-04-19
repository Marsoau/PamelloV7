using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Logging.Messages;
using PamelloV7.Framework.Logging.Services;
using PamelloV7.Framework.Modules.Loaders;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Logging;

public class PamelloLogger : IPamelloLogger
{
    private IPamelloModuleLoader? Modules { get; set; }

    internal void SetServices(IServiceProvider services) {
        Modules = services.GetRequiredService<IPamelloModuleLoader>();
    }
    
    public LogMessage Write(object? obj = null, ELogLevel level = ELogLevel.Log, Assembly? assembly = null) {
        assembly ??= Assembly.GetCallingAssembly();
        var module = Modules?.GetAssemblyModule(assembly);
        
        var message = new LogMessage() {
            Content = obj?.ToString() ?? "#null",
            Level = level,
            Module = module,
        };

        if (message.Level == ELogLevel.Debug) {
            Debug.WriteLine(message);
        }
        else {
            Console.WriteLine(message);
        }
        
        return message;
    }
}
