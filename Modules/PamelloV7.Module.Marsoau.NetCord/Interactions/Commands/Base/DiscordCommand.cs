using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;
using PamelloV7.Module.Marsoau.NetCord.Messages;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

public abstract class DiscordCommand : DiscordInteraction<SlashCommandInteraction>
{
    private IEventsService _events = null!;
    private UpdatableMessageService _updatableMessageService = null!;
    
    protected UpdatableMessage? UpdatableMessage;

    public override void Initialize(IServiceProvider services, SlashCommandInteraction interaction, IPamelloUser scopeUser) {
        base.Initialize(services, interaction, scopeUser);
        
        _events = services.GetRequiredService<IEventsService>();
        _updatableMessageService = services.GetRequiredService<UpdatableMessageService>();
    }

    public override Task RespondLoading() {
        if (HasResponded) return Task.CompletedTask;

        return RespondComponentAsync([
            Builder<BasicComponentsBuilder>().Loading()
        ]);
    }

    public Task RespondComponentAsync(IEnumerable<IMessageComponentProperties> components) {
        if (!HasResponded) HasResponded = true;
        else {
            return Interaction.ModifyResponseAsync(properties => properties.Components = components);
        }
        
        return Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties() {
            Components = components,
            Flags = MessageFlags.Ephemeral | MessageFlags.IsComponentsV2
        }));
    }
    
    private void ProcessUpdatableAsync(Func<IPamelloEntity?[]> getEntities) {
        if (UpdatableMessage is null) throw new Exception("Updatable message is not set on processing");

        var subscription = _events.Watch(async e => {
            await UpdatableMessage.Refresh();
        }, getEntities);

        UpdatableMessage.OnDead += () => {
            subscription.Dispose();
        };
    }
    
    public async Task RespondAsync(string content, Func<IPamelloEntity?[]>? entities = null) {
        entities ??= () => [];

        await RespondAsync(() => [
            Builder<BasicComponentsBuilder>().Info(null, content)
        ], entities);
    }
    public async Task RespondAsync(string header, string content, Func<IPamelloEntity?[]>? entities = null) {
        entities ??= () => [];

        await RespondAsync(() => [
            Builder<BasicComponentsBuilder>().Info(header, content)
        ], entities);
    }
    
    public Task<UpdatableMessage> RespondAsync(Func<IMessageComponentProperties> getContent, Func<IPamelloEntity?[]>? entities = null)
        => RespondAsync(() => [getContent()], entities);
    
    public async Task<UpdatableMessage> RespondAsync(Func<IEnumerable<IMessageComponentProperties>> getContent, Func<IPamelloEntity?[]>? entities = null) {
        entities ??= () => [];
        
        var needsRefresh = HasResponded;
        if (!needsRefresh) {
            await RespondComponentAsync(getContent());
        }

        UpdatableMessage = _updatableMessageService.Watch(new UpdatableMessage(this, 100,
            async _ => {
                await Interaction.ModifyResponseAsync(properties => properties.Components = getContent());
            }, async () => {
                await Interaction.DeleteResponseAsync();
            }
        ));
        
        ProcessUpdatableAsync(entities);
        
        if (needsRefresh) {
            await UpdatableMessage.Refresh();
        }
        
        return UpdatableMessage;
    }
}
