using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Commands;
using PamelloV7.Wrapper.Config;
using PamelloV7.Wrapper.Entities;
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

    public readonly IRemoteEntityQueryService PEQL;
    
    private bool _isSetup;

    public PamelloClient() {
        Config = new PamelloClientConfig();
        
        Events = new RemoteEventsService();
        
        Requests = new PamelloRequestsService(Config);
        Signal = new PamelloSignalService(Config, Events);
        Commands = new PamelloCommandsService(Requests, Signal);
        
        Users = new RemoteUserRepository(Requests);
        
        PEQL = new RemoteEntityQueryService(this);
    }

    private void Setup() {
        if (_isSetup) return;
        SafeStoredEntityStaticContainer.GetById = (type, id) => Users.Get(id);
        SafeStoredExtensions.GetSingleAsync = (type, id) => Users.GetSingleAsync(id);
    }

    public async Task ConnectAsync(string url) {
        if (Signal.IsConnected) throw new PamelloException("Already connected");
        if (!_isSetup) Setup();
        
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
