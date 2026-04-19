using System.Text;
using PamelloV7.Framework.Modules;

namespace PamelloV7.Framework.Logging.Messages;

public class LogMessage
{
    public string Content { get; init; }
    public DateTime TimeStamp { get; init; } = DateTime.Now;
    public ELogLevel Level { get; init; } = ELogLevel.Log;
    public IPamelloModule? Module { get; init; }

    public override string ToString() {
        return $"{TimeStamp:HH:mm:ss.fff} [{Module?.Name ?? "Server"} | {Level}] {Content}";
    }
}
