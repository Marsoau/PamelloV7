using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Commands;
using PamelloV7.Wrapper.Config;
using PamelloV7.Wrapper.Events.Services;
using PamelloV7.Wrapper.Requests;
using PamelloV7.Wrapper.Signal;

namespace PamelloV7.Wrapper;

public class PamelloClient
{
    private readonly PamelloClientConfig _config;
    
    public PamelloRequests Requests { get; }
    public PamelloSignal Signal { get; }
    public PamelloCommands Commands { get; }
    
    public RemoteEventsService Events { get; }

    public PamelloClient() {
        _config = new PamelloClientConfig();
        
        Requests = new PamelloRequests(_config);
        Signal = new PamelloSignal(_config, this);
        Commands = new PamelloCommands(Requests, Signal);
        Events = new RemoteEventsService();
    }

    public async Task ConnectAsync(string url) {
        if (Signal.IsConnected) throw new PamelloException("Already connected");
        
        _config.BaseUrl = url;
        
        await Signal.ConnectAsync();
    }
    
    public async Task<bool> AuthorizeAsync(Guid userToken) {
        if (!Signal.IsConnected) throw new PamelloException("Not connected");
        
        _config.Token = userToken;

        try {
            await Signal.AuthorizeAsync();
            return true;
        }
        catch {
            _config.Token = null;
            return false;
        }
    }

    public async Task DisconnectAsync() {
        if (!Signal.IsConnected) throw new PamelloException("Not connected");
        
        await Signal.DisconnectAsync();
    }
}
