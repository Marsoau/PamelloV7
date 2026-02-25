using System.Reflection;
using Microsoft.AspNetCore.SignalR;
using PamelloV7.Core.Dto.Signal;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Services;
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

    public async Task BroadcastMessageAsync(IPamelloUser? fromUser, IPamelloUser? toUser, string message) {
        if (toUser is null) {
            await _hub.Clients.All.SendAsync("Message", new SignalMessageDto(
                fromUser?.Id ?? 0,
                false,
                message
            ));
        }
        else foreach (var listener in _listeners.Where(x => x.Value == toUser)) {
            await _hub.Clients.Client(listener.Key).SendAsync("Message", new SignalMessageDto(
                fromUser?.Id ?? 0,
                true,
                message
            ));
        }
    }
    public void Broadcast(IPamelloEvent e) {
        BroadcastToPlayer(e, null);
    }

    public void BroadcastToPlayer(IPamelloEvent e, IPamelloPlayer? player) {
        List<EventTypeInfo> nestedTypes = [];

        var category = e.GetType().GetCustomAttribute<PamelloEventCategory>();
        var typeInfo = new EventTypeInfo(e.GetType().Name, category?.CustomCategory ?? "none");
        
        var currentBaseType = e.GetType().BaseType;
        while (currentBaseType is not null && currentBaseType != typeof(object)) {
            category = currentBaseType.GetCustomAttribute<PamelloEventCategory>();
            nestedTypes.Add(new EventTypeInfo(e.GetType().Name, category?.CustomCategory ?? "none"));
            currentBaseType = currentBaseType.BaseType;
        }
        
        foreach (var listener in _listeners.Where(x => x.Value is not null && (player is null || x.Value.SelectedPlayer == player))) {
            _hub.Clients.Client(listener.Key).SendAsync("Event", new EventSignalDto(
                typeInfo,
                nestedTypes,
                e
            )).Wait();
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

    public IPamelloUser GetRequiredUser(string connectionId)
        => GetUser(connectionId) ?? throw new HubException("You have to be authorized to send messages");
    public IPamelloUser? GetUser(string connectionId) {
        return _listeners.GetValueOrDefault(connectionId);
    }
}
