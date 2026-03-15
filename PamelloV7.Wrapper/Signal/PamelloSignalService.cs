using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PamelloV7.Core.Dto.Signal;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Commands;
using PamelloV7.Wrapper.Config;
using PamelloV7.Wrapper.Entities;
using PamelloV7.Wrapper.Events;
using PamelloV7.Wrapper.Events.Other;
using PamelloV7.Wrapper.Events.Services;
using PamelloV7.Wrapper.Exceptions;

namespace PamelloV7.Wrapper.Signal;

public class PamelloSignalService : IPamelloCommandInvoker
{
    private readonly PamelloClient _client;
    
    private HubConnection? _connection;

    protected HubConnection Connection =>
        _connection is not null && IsConnected ? _connection : throw new NotConnectedPamelloException("SignalR connection is not initiated");
    
    public RemoteUser? AuthorizedUser { get; private set; }
    
    public bool IsConnected => _connection?.State == HubConnectionState.Connected;
    public bool IsAuthorized => AuthorizedUser is not null;
    
    public event Action<bool>? OnConnected;
    public event Action<bool>? OnDisconnected;
    
    public event Action<bool>? OnAuthorized;
    public event Action<bool>? OnUnauthorized;
    
    public PamelloSignalService(PamelloClient client) {
        _client = client;

        _connection = null;
    }

    internal async Task<HubConnectionState> ConnectAsync(bool isAutomatic) {
        if (_client.Config.BaseUrl is null) throw new PamelloException("Base URL is not set");
        
        _connection = new HubConnectionBuilder()
            .WithUrl($"{_client.Config.BaseUrl}/Signal", options => {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            })
            .Build();

        _connection.On<ReceivedEventJsonDto>("Event", OnEvent);
        _connection.Closed += async error => {
            await _client.DisconnectAsync();
        };

        try {
            await _connection.StartAsync();
        }
        catch {
            await _connection.DisposeAsync();
            _connection = null;
            
            throw;
        }
        
        if (IsConnected) OnConnected?.Invoke(isAutomatic);

        return _connection.State;
    }

    private void OnEvent(ReceivedEventJsonDto eventDto) {
        Debug.WriteLine($"Received event: {string.Join("; ", eventDto.Types.Select(type => type.Name))}\n{eventDto.Data}");
        _client.Events.Invoke(eventDto);
    }

    internal async Task AuthorizeAsync(bool isAutomatic) {
        if (_client.Config.Token is null) throw new PamelloException("Token is not set");

        try {
            await Connection.InvokeAsync("Authorize", _client.Config.Token);
            AuthorizedUser = await _client.PEQL.GetSingleAsync<RemoteUser>("me");
            
            if (AuthorizedUser is not null) OnAuthorized?.Invoke(isAutomatic);
        }
        catch (Exception e) {
            AuthorizedUser = null;
            throw;
        }
    }
    
    internal async Task UnauthorizeAsync(bool isAutomatic) {
        if (!IsAuthorized) return;
        
        try {
            AuthorizedUser = null;
            
            if (IsConnected) await Connection.InvokeAsync("Unauthorize");
        }
        finally {
            _client.Config.Token = null;
        }
        
        OnUnauthorized?.Invoke(isAutomatic);
    }

    internal async Task DisconnectAsync(bool isAutomatic) {
        if (_connection is null) return;

        try {
            await _connection.StopAsync();
        }
        finally {
            await _connection.DisposeAsync();
            _connection = null;
        }
        
        OnDisconnected?.Invoke(isAutomatic);
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
