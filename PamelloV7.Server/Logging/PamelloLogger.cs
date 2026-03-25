using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Threading;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Logging.Messages;
using PamelloV7.Framework.Logging.Services;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Consolonia.Controls;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Logging;

public class PamelloLogger : IPamelloLogger
{
    private ConsoloniaService? Consolonia { get; set; }
    private IAssemblyTypeResolver? Types { get; set; }
    private IEventsService? Events { get; set; }

    public ObservableCollection<RefreshableLogMessage> Messages { get; set; } = [];

    internal void SetServices(IServiceProvider services) {
        Types = services.GetRequiredService<IAssemblyTypeResolver>();
        Events = services.GetRequiredService<IEventsService>();
        Consolonia = services.GetRequiredService<ConsoloniaService>();
    }
    
    public RefreshableLogMessage Write(object? obj = null, ELogLevel level = ELogLevel.Log, Assembly? assembly = null) {
        assembly ??= Assembly.GetCallingAssembly();
        var module = Types?.GetAssemblyModule(assembly);
        
        var message = new RefreshableLogMessage() {
            Content = obj?.ToString() ?? "#null",
            Level = level,
            Module = module,
        };

        if (ServerConfig.Root.UseConsolonia) {
            Dispatcher.UIThread.InvokeAsync(() => Messages.Add(message));
        }
        else {
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
        var module = Types?.GetAssemblyModule(assembly);
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
