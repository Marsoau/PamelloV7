using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Module.Marsoau.Discord.Services;

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
    
    public async Task ReleaseInteractionAsync() {
        var updatableMessageService = Services.GetRequiredService<UpdatableMessageKiller>();
        var updatableMessage = updatableMessageService.Get(Modal.Message.Id);
    
        updatableMessage?.Touch();
        
        await Modal.DeferAsync();
    }
    
    public TCommand Command<TCommand>()
        where TCommand : PamelloCommand, new()
    {
        var commands = Services.GetRequiredService<IPamelloCommandsService>();
        return commands.Get<TCommand>(User);
    }

    public string GetInputValue(string inputId) {
        var components = Modal.Data.Components.ToArray();
        var input = components.FirstOrDefault(component => component.CustomId == inputId);
        if (input is null) throw new PamelloException("No input found");
        
        return input.Value;
    }
    public string GetSelectValue(string selectId) {
        var components = Modal.Data.Components.ToArray();
        var select = components.FirstOrDefault(component => component.CustomId == selectId);
        if (select is null) throw new PamelloException("No select found");
        
        return select.Values.First();
    }
}
