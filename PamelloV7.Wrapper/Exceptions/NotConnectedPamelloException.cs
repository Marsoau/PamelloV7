namespace PamelloV7.Wrapper.Exceptions;

public class NotConnectedPamelloException : RemotePamelloException
{
    public NotConnectedPamelloException(string? message) : base(message) { }
}
