using System.Text;
using Avalonia.Threading;
using PamelloV7.Framework.Modules;

namespace PamelloV7.Framework.Logging.Messages;

public class RefreshableLogMessage
{
    public string Content {
        get => ContentBuilder.ToString();
        set => ContentBuilder.Clear().Append(value);
    }
    public StringBuilder ContentBuilder { get; init; } = new();
    public DateTime TimeStamp { get; init; } = DateTime.Now;
    public ELogLevel Level { get; init; } = ELogLevel.Log;
    public IPamelloModule? Module { get; init; }
    
    public event Action? OnRefresh;
    
    public virtual void Refresh() {
        if (OnRefresh is null) return;
        
        Dispatcher.UIThread.Invoke(OnRefresh.Invoke);
    }

    public override string ToString() {
        return $"{TimeStamp:HH:mm:ss.fff} [{Module?.Name ?? "Server"} | {Level}] {Content}";
    }
}
