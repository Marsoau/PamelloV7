using System.Text.Json;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using PamelloV7.Core.Dto.Signal;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Commands;
using PamelloV7.Wrapper.Config;
using PamelloV7.Wrapper.Events;
using PamelloV7.Wrapper.Events.Other;
using PamelloV7.Wrapper.Exceptions;

namespace PamelloV7.Wrapper.Signal;

public class PamelloSignal : IPamelloCommandInvoker
{
    private readonly PamelloClient _client;
    
    private HubConnection? _connection;

    protected HubConnection Connection => _connection is not null && IsConnected ? _connection : throw new NotConnectedPamelloException("SignalR connection is not initiated");
    
    public bool IsConnected => _connection?.State == HubConnectionState.Connected;
    
    public PamelloSignal(PamelloClient client) {
        _client = client;

        _connection = null;
    }

    internal async Task<HubConnectionState> ConnectAsync() {
        if (_client.Config.BaseUrl is null) throw new PamelloException("Base URL is not set");
        
        _connection = new HubConnectionBuilder()
            .WithUrl($"{_client.Config.BaseUrl}/Signal", options => {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<ReceivedEventJsonDto>("Event", OnEvent);
        
        await _connection.StartAsync();

        return _connection.State;
    }

    private void OnEvent(ReceivedEventJsonDto eventDto) {
        Console.WriteLine($"Received event: {eventDto.Type.Name} {eventDto.Data}");
        _client.Events.Invoke(eventDto);
    }

    internal async Task AuthorizeAsync() {
        if (_client.Config.Token is null) throw new PamelloException("Token is not set");
        
        await Connection.InvokeAsync("Authorize", _client.Config.Token);
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

    public async Task<string> ExecuteCommandPathAsync(string commandPath) {
        var result = await ExecuteCommandPathAsync<JsonElement?>(commandPath);
        if (result.HasValue) return result.Value.ToString();
        
        return string.Empty;
    }
    public Task<TType> ExecuteCommandPathAsync<TType>(string commandPath) {
        return Connection.InvokeAsync<TType>("Command", commandPath);
    }
}
