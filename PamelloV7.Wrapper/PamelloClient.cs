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

    public event Action? OnConnected;
    public event Action? OnDisconnected;
    
    public event Action? OnAuthorized;
    public event Action? OnUnauthorized;
    
    public event Action<Exception, int>? OnFailedAttempt;
    
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

        Signal.OnConnected += () => OnConnected?.Invoke();
        Signal.OnDisconnected += () => OnDisconnected?.Invoke();
        
        Signal.OnAuthorized += () => OnAuthorized?.Invoke();
        Signal.OnUnauthorized += () => OnUnauthorized?.Invoke();
        
        SafeStoredEntityStaticContainer.GetById = (type, id) => PEQL.GetSingle(type, id);
        SafeStoredExtensions.GetSingleAsyncFunc = (type, id) => PEQL.GetSingleAsync(type, id);
        SafeStoredExtensions.GetAsyncFunc = (type, query) => PEQL.GetAsync(type, query);
    }

    public async Task StartConnectionAttemptsAsync(string url, int maxAttempts = -1, int delay = 1000) {
        for (var attempt = 0; maxAttempts < 0 || attempt < maxAttempts; attempt++) {
            try {
                await ConnectAsync(url);
                return;
            }
            catch (Exception x) {
                OnFailedAttempt?.Invoke(x, attempt);
                await Task.Delay(delay);
            }
        }
    }
    
    public async Task ConnectAsync(string url) {
        if (Signal.IsConnected) throw new PamelloException("Already connected");
        
        Config.BaseUrl = url;
        
        await Signal.ConnectAsync();
    }
    
    public async Task<bool> AuthorizeAsync(Guid userToken) {
        if (!Signal.IsConnected) throw new PamelloException("Not connected");
        if (Signal.IsAuthorized) throw new PamelloException("Already authorized");
        
        Config.Token = userToken;

        try {
            await Signal.AuthorizeAsync();
        }
        catch {
            Config.Token = null;
        }
        
        return Signal.IsAuthorized;
    }
    
    public async Task UnauthorizeAsync() {
        await Signal.UnauthorizeAsync();
        
        PEQL.ClearCache();
    }

    public async Task DisconnectAsync() {
        await UnauthorizeAsync();
        await Signal.DisconnectAsync();
        
        Config.BaseUrl = null;
    }
}
