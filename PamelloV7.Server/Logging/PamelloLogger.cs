using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Threading;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Logging.Messages;
using PamelloV7.Framework.Logging.Services;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Consolonia.Controls;

namespace PamelloV7.Server.Logging;

public class PamelloLogger : IPamelloLogger
{
    internal IAssemblyTypeResolver? Types { get; set; }

    public ObservableCollection<RefreshableLogMessage> Messages { get; set; } = [];
    
    public RefreshableLogMessage Write(object? obj = null, ELogLevel level = ELogLevel.Log, Assembly? assembly = null) {
        assembly ??= Assembly.GetCallingAssembly();
        var module = Types?.GetAssemblyModule(assembly);
        var message = new RefreshableLogMessage() {
            Content = obj?.ToString() ?? "#null",
            Level = level,
            Module = module,
        };
        
        Dispatcher.UIThread.InvokeAsync(() => Messages.Add(message));
        
        return message;
    }
}
