using Microsoft.AspNetCore.SignalR;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services;
using PamelloV7.Server.Hubs;

namespace PamelloV7.Server.Services;

public class SignalBroadcastService : ISignalBroadcastService
{
    private readonly IServiceProvider _services;

    private readonly IHubContext<SignalHub> _hub;
    
    private readonly Dictionary<string, IPamelloUser?> _listeners;
    
    public SignalBroadcastService(IServiceProvider services) {
        _services = services;
        
        _hub = services.GetRequiredService<IHubContext<SignalHub>>();

        _listeners = [];
    }

    public async Task BroadcastMessageAsync(string message) {
        await _hub.Clients.All.SendAsync("Message", message);
    }
    public void Broadcast(IPamelloEvent e) {
        foreach (var listener in _listeners.Where(x => x.Value != null)) {
            _hub.Clients.Client(listener.Key).SendAsync("Event", new {
                Type = e.GetType().Name,
                Data = (object)e
            }).Wait();
        }
    }

    public void AddListener(string connectionId) {
        _listeners.Add(connectionId, null);
    }
    
    public void RemoveListener(string connectionId) {
        _listeners.Remove(connectionId);
    }
    
    public void AssignUser(string connectionId, IPamelloUser user) {
        if (!_listeners.ContainsKey(connectionId)) return;
        _listeners[connectionId] = user;
    }
    
    public void AbandonUser(string connectionId) {
        if (!_listeners.ContainsKey(connectionId)) return;
        _listeners[connectionId] = null;
    }

    public IPamelloUser? GetUser(string connectionId) {
        return _listeners.GetValueOrDefault(connectionId);
    }
}
