using PamelloV7.Wrapper.Requests;
using PamelloV7.Wrapper.Signal;

namespace PamelloV7.Wrapper.Commands;

public class PamelloCommandsService : IPamelloCommandInvoker
{
    public readonly PamelloRequestsService Requests;
    public readonly PamelloSignalService? Signal;
    
    public IPamelloCommandInvoker Invoker
        => Signal is { IsConnected: true } ? Signal : Requests;
    
    public PamelloCommandsService(PamelloRequestsService requests)
    {
        Requests = requests;
        Signal = null;
    }

    public PamelloCommandsService(PamelloRequestsService requests, PamelloSignalService signal) {
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
