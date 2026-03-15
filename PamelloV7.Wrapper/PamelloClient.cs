using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Commands;
using PamelloV7.Wrapper.Config;
using PamelloV7.Wrapper.Entities;
using PamelloV7.Wrapper.Entities.Base;
using PamelloV7.Wrapper.Events.Services;
using PamelloV7.Wrapper.Extensions;
using PamelloV7.Wrapper.Query;
using PamelloV7.Wrapper.Query.Base;
using PamelloV7.Wrapper.Repositories;
using PamelloV7.Wrapper.Repositories.Base;
using PamelloV7.Wrapper.Requests;
using PamelloV7.Wrapper.Signal;

namespace PamelloV7.Wrapper;

public enum EConnectionState
{
    Attempting,
    Connected,
    Authorized,
    Disconnected,
}

public class PamelloClient
{
    public readonly PamelloClientConfig Config;

    public readonly RemoteEventsService Events;

    public readonly PamelloRequestsService Requests;
    public readonly PamelloSignalService Signal;
    public readonly PamelloCommandsService Commands;

    public readonly RemoteUserRepository Users;
    public readonly RemoteSongRepository Songs;
    public readonly RemoteEpisodeRepository Episodes;
    public readonly RemotePlaylistRepository Playlists;
    public readonly RemotePlayerRepository Players;

    public readonly IRemoteEntityQueryService PEQL;

    public RemoteUser? User => Signal.AuthorizedUser;
    public RemoteUser RequiredUser => User ?? throw new PamelloException("Not authorized");
    
    public EConnectionState ConnectionState
        => _attemptsTask is not null ? EConnectionState.Attempting
            : Signal.IsAuthorized ? EConnectionState.Authorized
            : Signal.IsConnected ? EConnectionState.Connected
            : EConnectionState.Disconnected;
    
    public bool IsConnected => Signal.IsConnected;
    public bool IsAuthorized => Signal.IsAuthorized;
    
    public event Action? OnConnectionStateChanged;

    public event Action<bool>? OnConnected;
    public event Action<bool>? OnDisconnected;
    
    public event Action<bool>? OnAuthorized;
    public event Action<bool>? OnUnauthorized;
    
    public event Action<Exception, int>? OnFailedAttempt;
    
    private Task? _attemptsTask;
    private CancellationTokenSource? _attemptsCts;
    
    public PamelloClient() {
        Config = new PamelloClientConfig();
        
        Events = new RemoteEventsService(this);
        
        Requests = new PamelloRequestsService(Config);
        Signal = new PamelloSignalService(this);
        Commands = new PamelloCommandsService(Requests, Signal);
        
        Users = new RemoteUserRepository(Requests);
        Songs = new RemoteSongRepository(Requests);
        Episodes = new RemoteEpisodeRepository(Requests);
        Playlists = new RemotePlaylistRepository(Requests);
        Players = new RemotePlayerRepository(Requests);
        
        PEQL = new RemoteEntityQueryService(this);

        Signal.OnConnected += (isAutomatic) => {
            OnConnected?.Invoke(isAutomatic);
            OnConnectionStateChanged?.Invoke();
        };
        Signal.OnDisconnected += (isAutomatic) => {
            OnDisconnected?.Invoke(isAutomatic);
            OnConnectionStateChanged?.Invoke();
        };

        Signal.OnAuthorized += (isAutomatic) => {
            OnAuthorized?.Invoke(isAutomatic);
            OnConnectionStateChanged?.Invoke();
        };
        Signal.OnUnauthorized += (isAutomatic) => {
            OnUnauthorized?.Invoke(isAutomatic);
            OnConnectionStateChanged?.Invoke();
        };
        
        SafeStoredEntityStaticContainer.GetById = (type, id) => PEQL.GetSingle(type, id);
        SafeStoredExtensions.GetSingleAsyncFunc = (type, id) => PEQL.GetSingleAsync(type, id);
        SafeStoredExtensions.GetAsyncFunc = (type, query) => PEQL.GetAsync(type, query);
    }

    public async Task StopConnectionAttemptsAsync() {
        var task = _attemptsCts?.CancelAsync();
        if (task is not null) await task;
    }
    public async Task StartConnectionAttemptsAsync(string url, int maxAttempts = -1, int delay = 1000) {
        if (_attemptsTask is not null) {
            await _attemptsTask;
            return;
        }
        
        _attemptsTask = StartConnectionAttemptsInternalAsync(url, maxAttempts, delay);
        OnConnectionStateChanged?.Invoke();

        try {
            await _attemptsTask;
        }
        finally {
            _attemptsTask = null;
            
            _attemptsCts?.Dispose();
            _attemptsCts = null;
            
            OnConnectionStateChanged?.Invoke();
        }
    }
    public async Task StartConnectionAttemptsInternalAsync(string url, int maxAttempts = -1, int delay = 1000, CancellationToken cancellationToken = default) {
        _attemptsCts = new CancellationTokenSource();
        
        for (var attempt = 0; maxAttempts < 0 || attempt < maxAttempts && !_attemptsCts.Token.IsCancellationRequested; attempt++) {
            try {
                await ConnectAsync(url, true);
                break;
            }
            catch (Exception x) {
                OnFailedAttempt?.Invoke(x, attempt);
                await Task.Delay(delay, cancellationToken);
            }
        }
        
        _attemptsCts.Dispose();
        _attemptsCts = null;
    }
    
    internal async Task ConnectAsync(string url, bool isAutomatic = false) {
        if (Signal.IsConnected) throw new PamelloException("Already connected");
        
        Config.BaseUrl = url;
        
        await Signal.ConnectAsync(isAutomatic);
    }
    
    public async Task<bool> AuthorizeAsync(Guid userToken, bool isAutomatic = false) {
        if (!Signal.IsConnected) throw new PamelloException("Not connected");
        if (Signal.IsAuthorized) throw new PamelloException("Already authorized");
        
        Config.Token = userToken;

        try {
            await Signal.AuthorizeAsync(isAutomatic);
        }
        catch {
            Config.Token = null;
        }
        
        return Signal.IsAuthorized;
    }
    
    public async Task UnauthorizeAsync(bool isAutomatic = false) {
        await Signal.UnauthorizeAsync(isAutomatic);
        
        PEQL.ClearCache();
    }

    public async Task DisconnectAsync(bool isAutomatic = false) {
        await UnauthorizeAsync(isAutomatic);
        await Signal.DisconnectAsync(isAutomatic);
        
        Config.BaseUrl = null;
    }
}
