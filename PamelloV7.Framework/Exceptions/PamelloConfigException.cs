using PamelloV7.Core.Exceptions;

namespace PamelloV7.Framework.Exceptions;

public class PamelloConfigException : PamelloException
{
    public PamelloConfigException(string? message) : base(message) { }
}
