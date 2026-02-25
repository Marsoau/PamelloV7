using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Commands;
using PamelloV7.Wrapper.Config;
using PamelloV7.Wrapper.Exceptions;

namespace PamelloV7.Wrapper;

public class PamelloSignal : IPamelloCommandInvoker
{
    private readonly PamelloClientConfig _config;
    
    private HubConnection? _connection;

    protected HubConnection Connection => _connection is not null && IsConnected ? _connection : throw new NotConnectedPamelloException("SignalR connection is not initiated");
    
    public bool IsConnected => _connection?.State == HubConnectionState.Connected;
    
    public PamelloSignal(PamelloClientConfig config) {
        _config = config;

        _connection = null;
    }

    internal async Task<HubConnectionState> ConnectAsync() {
        if (_config.BaseUrl is null) throw new PamelloException("Base URL is not set");
        
        _connection = new HubConnectionBuilder()
            .WithUrl($"{_config.BaseUrl}/Signal")
            .WithAutomaticReconnect()
            .Build();
        
        await _connection.StartAsync();

        return _connection.State;
    }

    internal async Task AuthorizeAsync() {
        if (_config.Token is null) throw new PamelloException("Token is not set");
        
        await Connection.InvokeAsync("Authorize", _config.Token);
    }

    internal async Task DisconnectAsync() {
        if (_connection is null) return;
        
        await _connection.StopAsync();
        await _connection.DisposeAsync();
        
        _connection = null;
    }
    
    public async Task SendMessage(string message) {
        await Connection.InvokeAsync("Message", message);
    }

    public async Task<string> ExecuteCommandAsync(string commandPath) {
        var result = await ExecuteCommandAsync<JsonElement?>(commandPath);
        if (result.HasValue) return result.Value.ToString();
        
        return string.Empty;
    }
    public Task<TType> ExecuteCommandAsync<TType>(string commandPath) {
        return Connection.InvokeAsync<TType>("Command", commandPath);
    }
}
