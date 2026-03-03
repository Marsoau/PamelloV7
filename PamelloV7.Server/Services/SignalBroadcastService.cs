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
        List<EventTypeInfo> types = [];

        
        var currentType = e.GetType();
        while (currentType is not null && currentType != typeof(object)) {
            IEntityInfoUpdateAttribute? infoUpdateAttribute = null;
            var category = currentType.GetCustomAttribute<PamelloEventCategory>();
            var infoUpdateAttributeData = currentType.GetCustomAttributesData()
                .FirstOrDefault(attr => attr.AttributeType.IsGenericType 
                                        && attr.AttributeType.GetGenericTypeDefinition() == typeof(EntityInfoUpdateAttribute<>)
                );
            if (infoUpdateAttributeData is not null && currentType.GetCustomAttribute(infoUpdateAttributeData.AttributeType) is IEntityInfoUpdateAttribute attribute) infoUpdateAttribute = attribute;
            
            var entityTypeName = infoUpdateAttribute?.EntityType.Name ?? "";
            var entityPropertyName = infoUpdateAttribute?.EntityPropertyName ?? "";
            var updatePropertyName = string.Join(".", infoUpdateAttribute?.PropertyPath ?? []);
            
            types.Add(new EventTypeInfo(
                currentType.Name,
                category?.CustomCategory ?? "none",
                entityTypeName,
                entityPropertyName,
                updatePropertyName
            ));
            currentType = currentType.BaseType;
        }
        
        foreach (var listener in _listeners.Where(x => x.Value is not null && (player is null || x.Value.SelectedPlayer == player))) {
            _hub.Clients.Client(listener.Key).SendAsync("Event", new EventSignalDto(
                types,
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
