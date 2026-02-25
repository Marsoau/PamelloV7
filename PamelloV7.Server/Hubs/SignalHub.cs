using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Converters;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Hubs;

public class SignalHub : Hub
{
    private readonly IServiceProvider _services;
    
    private readonly IPamelloCommandsService _commands;

    private readonly SignalBroadcastService _broadcast;
    
    public SignalHub(IServiceProvider services) {
        _services = services;
        
        _commands = services.GetRequiredService<IPamelloCommandsService>();
        
        _broadcast = (SignalBroadcastService)services.GetRequiredService<ISignalBroadcastService>();
    }
    
    public override async Task OnConnectedAsync() {
        Console.WriteLine($"Connected: {Context.ConnectionId}");
        
        _broadcast.AddListener(Context.ConnectionId);
        await _broadcast.BroadcastMessageAsync(null, null, $"Connected new {Context.ConnectionId} client");
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        Console.WriteLine($"Disconnected: {Context.ConnectionId}");
        
        _broadcast.RemoveListener(Context.ConnectionId);
        await _broadcast.BroadcastMessageAsync(null, null, $"Client {Context.ConnectionId} disconnected");
    }

    public async Task Authorize(string userToken) {
        if (!Guid.TryParse(userToken, out var token)) {
            throw new PamelloException("Invalid token format");
        }
        
        var users = _services.GetRequiredService<IPamelloUserRepository>();
        var user = users.GetByToken(token);
        
        if (user is null) throw new PamelloException("User not found");
        
        await _broadcast.BroadcastMessageAsync(null, null, $"Client {Context.ConnectionId} authorized as {user}");
        
        _broadcast.AssignUser(Context.ConnectionId, user);
    }
    public async Task Unauthorize() {
        if (_broadcast.GetUser(Context.ConnectionId) is null) return;
        
        await _broadcast.BroadcastMessageAsync(null, null, $"Client {Context.ConnectionId} unauthorized");
        
        _broadcast.AbandonUser(Context.ConnectionId);
    }

    public async Task Message(string message) {
        var user = _broadcast.GetRequiredUser(Context.ConnectionId);
        
        await _broadcast.BroadcastMessageAsync(user, null, message);
    }

    public async Task<object?> Command(string commandPath) {
        var user = _broadcast.GetUser(Context.ConnectionId);
        if (user is null) throw new HubException("You have to be authorized to execute commands");
        
        await _broadcast.BroadcastMessageAsync(null, null, $"Client {Context.ConnectionId} of user {user} executed: {commandPath}");

        try {
            return await _commands.ExecutePathAsync(commandPath, user);
        }
        catch (PamelloException pamelloException) {
            throw new HubException(pamelloException.Message);
        }
    }
}
