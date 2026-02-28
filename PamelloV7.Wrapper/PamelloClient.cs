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

    public readonly IRemoteEntityQueryService PEQL;
    
    public PamelloClient() {
        Config = new PamelloClientConfig();
        
        Events = new RemoteEventsService();
        
        Requests = new PamelloRequestsService(Config);
        Signal = new PamelloSignalService(Config, Events);
        Commands = new PamelloCommandsService(Requests, Signal);
        
        Users = new RemoteUserRepository(Requests);
        Songs = new RemoteSongRepository(Requests);
        Episodes = new RemoteEpisodeRepository(Requests);
        Playlists = new RemotePlaylistRepository(Requests);
        
        PEQL = new RemoteEntityQueryService(this);
        
        SafeStoredEntityStaticContainer.GetById = (type, id) => PEQL.GetSingle(type, id);
        SafeStoredExtensions.GetSingleAsyncFunc = (type, id) => PEQL.GetSingleAsync(type, id);
        SafeStoredExtensions.GetAsyncFunc = (type, query) => PEQL.GetAsync(type, query);
    }

    public async Task ConnectAsync(string url) {
        if (Signal.IsConnected) throw new PamelloException("Already connected");
        
        Config.BaseUrl = url;
        
        await Signal.ConnectAsync();
    }
    
    public async Task<bool> AuthorizeAsync(Guid userToken) {
        if (!Signal.IsConnected) throw new PamelloException("Not connected");
        
        Config.Token = userToken;

        try {
            await Signal.AuthorizeAsync();
            return true;
        }
        catch {
            Config.Token = null;
            return false;
        }
    }

    public async Task DisconnectAsync() {
        if (!Signal.IsConnected) throw new PamelloException("Not connected");
        
        await Signal.DisconnectAsync();
    }
}
