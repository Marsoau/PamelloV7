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
using PamelloV7.Module.Marsoau.NetCord.Config;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;
using PamelloV7.Module.Marsoau.NetCord.Messages;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

public abstract class DiscordCommand : DiscordInteraction<SlashCommandInteraction>
{
    public delegate IEnumerable<IMessageComponentProperties?> GetContent();
    public delegate IEnumerable<IMessageComponentProperties?> GetContentForOne<TType>(TType item);
    public delegate IEnumerable<IMessageComponentProperties?> GetPageContent(int page);
    
    public delegate Task<IEnumerable<IMessageComponentProperties?>> GetContentAsync();
    public delegate Task<IEnumerable<IMessageComponentProperties?>> GetContentForOneAsync<TType>(TType item);
    public delegate Task<IEnumerable<IMessageComponentProperties?>> GetPageContentAsync(int page);
    
    private IEventsService _events = null!;
    private UpdatableMessageService _updatableMessageService = null!;
    
    protected UpdatableMessage? UpdatableMessage;

    public override void InitializeInteraction(IServiceProvider services, SlashCommandInteraction interaction, IPamelloUser scopeUser) {
        base.InitializeInteraction(services, interaction, scopeUser);
        
        _events = services.GetRequiredService<IEventsService>();
        _updatableMessageService = services.GetRequiredService<UpdatableMessageService>();
    }

    protected override Task RespondLoadingInternal() {
        if (HasResponded) return Task.CompletedTask;

        return RespondComponentAsync([
            Builder<BasicComponentsBuilder>().Loading()
        ]);
    }

    protected override Task ReleaseInteractionInternal() {
        return Task.CompletedTask;
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
    
    private void ProcessUpdatableAsync(GetEntities getEntities) {
        if (UpdatableMessage is null) throw new Exception("Updatable message is not set on processing");

        var subscription = _events.Watch(async e => {
            await UpdatableMessage.Refresh();
        }, getEntities);

        UpdatableMessage.OnDead += () => {
            subscription.Dispose();
        };
    }

    public Task RespondAsync(string content, GetEntities? entities = null)
        => RespondAsync(() => null, () => content, entities);
    public Task RespondAsync(string header, string content, GetEntities? entities = null)
        => RespondAsync(() => header, () => content, entities);
    
    public Task RespondAsync(string header, Func<string?> getContent, GetEntities? entities = null)
        => RespondAsync(() => header, getContent, entities);
    
    public async Task RespondAsync(Func<string?> getContent, GetEntities? entities = null)
        => await RespondAsync(() => null, getContent, entities);
    public async Task RespondAsync(Func<string?> getHeader, Func<string?> getContent, GetEntities? entities = null) {
        entities ??= () => [];

        await RespondAsync(() => [
            Builder<BasicComponentsBuilder>().Info(getHeader(), getContent())
        ], entities);
    }
    
    public Task<UpdatableMessage> RespondAsync(Func<IMessageComponentProperties?> getContent, GetEntities? entities = null)
        => RespondAsync(() => [getContent()], entities);
    
    public Task<UpdatableMessage> RespondAsync(Func<IEnumerable<IMessageComponentProperties?>> getContent, GetEntities? entities = null)
        => RespondAsync(() => Task.FromResult(getContent()), entities);
    public async Task<UpdatableMessage> RespondAsync(Func<Task<IEnumerable<IMessageComponentProperties?>>> getContent, GetEntities? entities = null) {
        entities ??= () => [];
        
        var needsRefresh = HasResponded;
        if (!needsRefresh) {
            await RespondComponentAsync((await getContent()).OfType<IMessageComponentProperties>());
        }

        UpdatableMessage = _updatableMessageService.Watch(new UpdatableMessage(this, NetCordConfig.Root.Commands.UpdatableCommandsLifetime,
            async message => {
                await Interaction.ModifyResponseAsync(properties => properties.Components = getContent().Result.OfType<IMessageComponentProperties>());
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

    public async Task<UpdatablePageMessage> RespondPageAsync(
        GetPageContent getPageContent, GetEntities entities)
        => await RespondPageAsync(page => Task.FromResult(getPageContent(page)), entities);
    public async Task<UpdatablePageMessage> RespondPageAsync(GetPageContentAsync getPageContentAsync, GetEntities entities) {
        var needsRefresh = HasResponded;
        if (!needsRefresh) {
            await RespondComponentAsync((await getPageContentAsync(0)).OfType<IMessageComponentProperties>());
        }
        
        UpdatableMessage = _updatableMessageService.Watch(new UpdatablePageMessage(this, NetCordConfig.Root.Commands.UpdatableCommandsLifetime,
            async updatableMessage => {
                if (updatableMessage is not UpdatablePageMessage updatablePageMessage) return;
                
                await Interaction.ModifyResponseAsync(properties => properties.Components = getPageContentAsync(updatablePageMessage.Page).Result.OfType<IMessageComponentProperties>());
            }, async () => {
                await Interaction.DeleteResponseAsync();
            }
        ));
        
        ProcessUpdatableAsync(entities);

        if (needsRefresh) {
            await UpdatableMessage.Refresh();
        }
        
        return (UpdatablePageMessage)UpdatableMessage;
    }

    public Task RespondOneOrManyAsync<TType>
    (
        List<TType> items,
        GetContentForOne<TType> getContentForOne,
        string manyItemsTitle,
        GetEntities? getEntities = null
    ) where TType : IPamelloEntity
        => RespondOneOrManyAsync(
            items,
            item => Task.FromResult(getContentForOne(item)),
            manyItemsTitle,
            getEntities
        );

    public Task RespondOneOrManyAsync<TType>
    (
        List<TType> items,
        GetContentForOneAsync<TType> getContentForOneAsync,
        string manyItemsTitle,
        GetEntities? getEntities = null
    ) where TType : IPamelloEntity
        => RespondOneOrManyAsync(
            items,
            getContentForOneAsync,
            page => 
                Builder<BasicComponentsBuilder>()
                    .EntitiesList(
                        manyItemsTitle,
                        items.OfType<IPamelloEntity>(),
                        page
                    )
            ,
            getEntities
        );
    public Task RespondOneOrManyAsync<TType>(
        List<TType> items,
        GetContentForOne<TType> getContentForOne,
        GetPageContent getContentForMany,
        GetEntities? getEntities = null
    ) where TType : IPamelloEntity
        => RespondOneOrManyAsync(
            items,
            item => Task.FromResult(getContentForOne(item)),
            page => Task.FromResult(getContentForMany(page)),
            getEntities
        );
    
    public Task RespondOneOrManyAsync<TType>(
        List<TType> items,
        GetContentForOneAsync<TType> getContentForOneAsync,
        GetPageContent getContentForMany,
        GetEntities? getEntities = null
    ) where TType : IPamelloEntity
        => RespondOneOrManyAsync(
            items,
            getContentForOneAsync,
            page => Task.FromResult(getContentForMany(page)),
            getEntities
        );
    
    public Task RespondOneOrManyAsync<TType>(
        List<TType> items,
        GetContentForOne<TType> getContentForOne,
        GetPageContentAsync getContentForManyAsync,
        GetEntities? getEntities = null
    ) where TType : IPamelloEntity
        => RespondOneOrManyAsync(
            items,
            item => Task.FromResult(getContentForOne(item)),
            getContentForManyAsync,
            getEntities
        );
    
    public async Task RespondOneOrManyAsync<TType>(
        List<TType> items,
        GetContentForOneAsync<TType> getContentForOneAsync,
        GetPageContentAsync getContentForManyAsync,
        GetEntities? getEntities = null
    ) 
        where TType : IPamelloEntity
    {
        getEntities ??= () => [..items];
        
        if (items.Count == 1) {
            await RespondAsync(() => getContentForOneAsync(items.First()), getEntities);
        }
        else {
            await RespondPageAsync(getContentForManyAsync, getEntities);
        }
    }
}
