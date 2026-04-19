using System.Reflection;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging.Messages;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Logging.Services;

public interface IPamelloLogger
{
    public LogMessage Write(
        object? obj = null,
        ELogLevel level = ELogLevel.Log,
        Assembly? assembly = null
    );
}
