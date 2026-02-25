namespace PamelloV7.Wrapper.Commands;

public class PamelloCommands
{
    public readonly PamelloRequests Requests;
    public readonly PamelloSignal? Signal;
    
    public IPamelloCommandInvoker Invoker
        => Signal is { IsConnected: true } ? Signal : Requests;
    
    public PamelloCommands(PamelloRequests requests, PamelloSignal? signal = null)
    {
        Requests = requests;
        Signal = signal;
    }
}
