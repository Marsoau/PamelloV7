using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Module.Marsoau.Discord.Context;
using PamelloV7.Module.Marsoau.Discord.Messages;
using DiscordConfig = PamelloV7.Module.Marsoau.Discord.Config.DiscordConfig;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Base;

public class DiscordCommand : InteractionModuleBase<PamelloSocketInteractionContext>
{
    public IServiceProvider Services => Context.Services;

    public TCommand Command<TCommand>()
        where TCommand : PamelloCommand, new()
    {
        var commands = Services.GetRequiredService<IPamelloCommandsService>();
        return commands.Get<TCommand>(Context.User);
    }
    
    public async Task<UpdatableMessage> RespondUpdatableAsync(Action<MessageProperties> editMessage, params IPamelloEntity[] entities) {
        await DeferAsync(ephemeral: true);
        
        var events = Services.GetRequiredService<IEventsService>();
        
        var message = await ModifyOriginalResponseAsync(editMessage);
        var updatableMessage = new UpdatableMessage(message, new AudioTime(DiscordConfig.Root.Commands.UpdatableCommandsLifetime),
            async () => {
                await ModifyOriginalResponseAsync(editMessage);
            }
        );

        var subscription = events.Watch(async (e) => {
            Console.WriteLine("ENTITY WATCH EXISTS");
            await updatableMessage.Refresh();
        }, entities);

        Task.Run(async () => {
            await updatableMessage.Lifetime;
            
            updatableMessage.Dispose();
            subscription.Dispose();
            
            await DeleteOriginalResponseAsync();
        });
        
        return updatableMessage;
    }
}
