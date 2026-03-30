using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using Avalonia.Threading;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Consolonia;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Logging.Messages;
using PamelloV7.Framework.Logging.Services;
using PamelloV7.Framework.Modules.Loaders;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Consolonia.Controls;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Logging;

public class PamelloLogger : IPamelloLogger
{
    private IConsoloniaService? Consolonia { get; set; }
    private IPamelloModuleLoader? Modules { get; set; }
    private IEventsService? Events { get; set; }

    public ObservableCollection<RefreshableLogMessage> Messages { get; set; } = [];

    internal void SetServices(IServiceProvider services) {
        Modules = services.GetRequiredService<IPamelloModuleLoader>();
        Events = services.GetRequiredService<IEventsService>();
        Consolonia = services.GetRequiredService<IConsoloniaService>();
    }
    
    public RefreshableLogMessage Write(object? obj = null, ELogLevel level = ELogLevel.Log, Assembly? assembly = null) {
        assembly ??= Assembly.GetCallingAssembly();
        var module = Modules?.GetAssemblyModule(assembly);
        
        var message = new RefreshableLogMessage() {
            Content = obj?.ToString() ?? "#null",
            Level = level,
            Module = module,
        };

        if (Consolonia is not null && Consolonia.IsAvailable) {
            Dispatcher.UIThread.InvokeAsync(() => Messages.Add(message));
        }
        else if (message.Level == ELogLevel.Debug) {
            Debug.WriteLine(message);
        }
        else if (!ServerConfig.Root.UseConsolonia) {
            Console.WriteLine(message);
        }
            
        return message;
    }

    public RefreshableLogMessage Write(
        Func<string> getContent,
        Func<IPamelloEntity?[]> getEntities,
        ELogLevel level = ELogLevel.Log,
        Assembly? assembly = null
    ) {
        if (Events is null) {
            throw new Exception("Events service is required for updatable messages");
        }
        
        assembly ??= Assembly.GetCallingAssembly();
        var module = Modules?.GetAssemblyModule(assembly);
        var message = new UpdatableLogMessage(getContent) {
            Level = level,
            Module = module,
        };
        
        Events.Watch(_ => {
            message.Refresh();
            return Task.CompletedTask;
        }, getEntities);

        if (ServerConfig.Root.UseConsolonia) {
            Dispatcher.UIThread.InvokeAsync(() => Messages.Add(message));
        }
        
        return message;
    }
}
