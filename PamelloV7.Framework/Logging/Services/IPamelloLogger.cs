using System.Reflection;
using PamelloV7.Framework.Logging.Messages;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Logging.Services;

public interface IPamelloLogger
{
    public RefreshableLogMessage Write(object? obj = null, ELogLevel level = ELogLevel.Log, Assembly? assembly = null);
}
