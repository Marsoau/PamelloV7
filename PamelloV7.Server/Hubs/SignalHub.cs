using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using PamelloV7.Core.Converters;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
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
        await _broadcast.BroadcastMessageAsync($"Connected new {Context.ConnectionId} client");
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        Console.WriteLine($"Disconnected: {Context.ConnectionId}");
        
        _broadcast.RemoveListener(Context.ConnectionId);
        await _broadcast.BroadcastMessageAsync($"Client {Context.ConnectionId} disconnected");
    }

    public async Task Authorize(string userId) {
        var users = _services.GetRequiredService<IPamelloUserRepository>();
        var user = users.GetRequired(int.Parse(userId));
        
        await _broadcast.BroadcastMessageAsync($"Client {Context.ConnectionId} authorized as {user}");
        
        _broadcast.AssignUser(Context.ConnectionId, user);
    }
    public async Task Unauthorize() {
        await _broadcast.BroadcastMessageAsync($"Client {Context.ConnectionId} unauthorized");
        
        _broadcast.AbandonUser(Context.ConnectionId);
    }

    public async Task<object?> Command(string commandPath) {
        var user = _broadcast.GetUser(Context.ConnectionId);
        if (user is null) throw new PamelloException("You have to be authorized to execute commands");

        if (user.SelectedAuthorization?.Info is null) user.SelectedAuthorization!.UpdateInfo();
        
        await _broadcast.BroadcastMessageAsync($"Client {Context.ConnectionId} of user {user} executed: {commandPath}");
        
        return await _commands.ExecuteAsync(commandPath, user);
    }
}
