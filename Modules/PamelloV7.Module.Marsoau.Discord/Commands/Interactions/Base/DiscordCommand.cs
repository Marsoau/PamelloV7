using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Context;
using PamelloV7.Module.Marsoau.Discord.Messages;
using PamelloV7.Module.Marsoau.Discord.Services;
using DiscordConfig = PamelloV7.Module.Marsoau.Discord.Config.DiscordConfig;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Base;

public abstract class DiscordCommand : InteractionModuleBase<PamelloSocketInteractionContext>
{
    protected UpdatableMessage? _updatableMessage;
    
    public IServiceProvider Services => Context.Services;

    private IEntityQueryService? __peql;
    public IEntityQueryService _peql =>
        __peql ??= Services.GetRequiredService<IEntityQueryService>();

    public TCommand Command<TCommand>()
        where TCommand : PamelloCommand, new()
    {
        var commands = Services.GetRequiredService<IPamelloCommandsService>();
        return commands.Get<TCommand>(Context.User);
    }

    public Task RespondInfo(string title, string description) {
        return RespondAsync(components: PamelloComponentBuilders.Info(title, description).Build(), ephemeral: true);
    }

    public async Task EndInteractionAsync() {
        if (Context.Interaction is SocketMessageComponent component)
        {
            await component.UpdateAsync(msg => { });
        }
    }

    public Task<UpdatableMessage> RespondUpdatableAsync(Action<MessageProperties> editMessage, params IPamelloEntity[] entities) {
        return RespondUpdatableAsync(editMessage, () => entities);
    }
    public async Task<UpdatableMessage> RespondUpdatableAsync(Action<MessageProperties> editMessage, Func<IPamelloEntity[]> entities) {
        await RespondAsync(components: PamelloComponentBuilders.Defer().Build(), ephemeral: true);
        
        var events = Services.GetRequiredService<IEventsService>();
        var updatableMessageService = Services.GetRequiredService<UpdatableMessageKiller>();
        
        var message = await ModifyOriginalResponseAsync(editMessage);
        _updatableMessage = updatableMessageService.Watch(new UpdatableMessage(message, new AudioTime(DiscordConfig.Root.Commands.UpdatableCommandsLifetime),
            async () => {
                await ModifyOriginalResponseAsync(editMessage);
            }, async () => {
                await DeleteOriginalResponseAsync();
            }
        ));

        var subscription = events.Watch(async (e) => {
            Console.WriteLine("ENTITY WATCH EXISTS");
            await _updatableMessage.Refresh();
        }, entities);

        _updatableMessage.OnDead += () => {
            subscription.Dispose();
        };
        
        return _updatableMessage;
    }
}
