using PamelloV7.Wrapper.Requests;
using PamelloV7.Wrapper.Signal;

namespace PamelloV7.Wrapper.Commands;

public class PamelloCommands : IPamelloCommandInvoker
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

    public Task<string> ExecuteCommandPathAsync(string commandPath) {
        return Invoker.ExecuteCommandPathAsync(commandPath);
    }

    public Task<TType> ExecuteCommandPathAsync<TType>(string commandPath) {
        return Invoker.ExecuteCommandPathAsync<TType>(commandPath);
    }
}
