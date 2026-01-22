using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Modals.Base;

public abstract class DiscordModal
{
    public IServiceProvider Services;
    public SocketModal Modal;

    public IPamelloUser User
        => Services.GetRequiredService<IPamelloUserRepository>().GetByPlatformKey(new PlatformKey("discord", Modal.User.Id.ToString()))!;
    
    private IEntityQueryService? __peql;
    public IEntityQueryService _peql =>
        __peql ??= Services.GetRequiredService<IEntityQueryService>();
    
    public abstract Task Submit(string args);
    
    public Task EndInteraction() {
        return Modal.UpdateAsync(o => { });
    }
    
    public TCommand Command<TCommand>()
        where TCommand : PamelloCommand, new()
    {
        var commands = Services.GetRequiredService<IPamelloCommandsService>();
        return commands.Get<TCommand>(User);
    }
}
