using System.Text;
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
    
    public void Refresh() {
        throw new NotImplementedException();
    }
}
