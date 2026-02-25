using PamelloV7.Core.Exceptions;

namespace PamelloV7.Wrapper.Exceptions;

public class RemotePamelloException : PamelloException
{
    public RemotePamelloException(string? message) : base(message) { }
}
