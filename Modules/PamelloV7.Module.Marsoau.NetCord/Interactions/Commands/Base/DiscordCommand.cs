using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;
using PamelloV7.Module.Marsoau.NetCord.Messages;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

public abstract class DiscordCommand : DiscordInteraction<SlashCommandInteraction>
{
    private IEventsService Events { get; set; } = null!;
    private UpdatableMessageService UpdatableMessageService { get; set; } = null!;
    
    protected UpdatableMessage? _updatableMessage;

    public override void Initialize(IServiceProvider services, SlashCommandInteraction interaction, IPamelloUser scopeUser) {
        base.Initialize(services, interaction, scopeUser);
        
        Events = services.GetRequiredService<IEventsService>();
        UpdatableMessageService = services.GetRequiredService<UpdatableMessageService>();
    }

    public override Task RespondLoading() {
        if (HasResponded) return Task.CompletedTask;

        return RespondTextAsync("loading");
    }

    public Task RespondTextAsync(string content) {
        return Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties() {
            Content = content,
            Flags = MessageFlags.Ephemeral
        }));
    }
    
    public Task RespondAsync(string content)
        => RespondAsync(() => content, () => []);

    private void ProcessUpdatableAsync(Func<IPamelloEntity?[]> getEntities) {
        if (_updatableMessage is null) throw new Exception("Updatable message is not set on processing");

        var subscription = Events.Watch(async e => {
            await _updatableMessage.Refresh();
        }, getEntities);

        _updatableMessage.OnDead += () => {
            subscription.Dispose();
        };
    }
    
    public async Task<UpdatableMessage> RespondAsync(Func<string> getContent, Func<IPamelloEntity?[]>? entities = null) {
        entities ??= () => [];
        
        var needsRefresh = HasResponded;
        if (!needsRefresh) {
            await RespondTextAsync(getContent());
        }

        _updatableMessage = UpdatableMessageService.Watch(new UpdatableMessage(this, 100,
            async _ => {
                await Interaction.ModifyResponseAsync(properties => properties.Content = getContent());
            }, async () => {
                await Interaction.DeleteResponseAsync();
            }
        ));
        
        ProcessUpdatableAsync(entities);
        
        if (needsRefresh) {
            await _updatableMessage.Refresh();
        }
        
        return _updatableMessage;
    }
}
