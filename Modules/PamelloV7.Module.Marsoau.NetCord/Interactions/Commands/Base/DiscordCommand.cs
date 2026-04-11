using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Attributes.Variants;
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

public abstract partial class DiscordCommand : DiscordInteraction<SlashCommandInteraction>
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

    public bool HasResponded {
        set; get => field || RespondedWithLoading;
    }

    public DiscordCommand? ParentCommand { get; set; }
    
    public List<DiscordInteraction> CommandsInteractions = null!;
    public int CommandInteractionIndex;

    public virtual void InitializeCommand(
        Interaction interaction,
        DiscordCommand? parentCommand,
        IServiceProvider services,
        IPamelloUser scopeUser
    ) {
        InitializeInteraction(interaction, services, scopeUser);
        
        ParentCommand = parentCommand;

        if (ParentCommand is null) {
            CommandsInteractions = [this];
            
            CommandInteractionIndex = 0;
        }
        else {
            CommandsInteractions = ParentCommand.CommandsInteractions;
            CommandsInteractions.Add(this);
            
            CommandInteractionIndex = CommandsInteractions.Count - 1;
        }
        
        _events = services.GetRequiredService<IEventsService>();
        _updatableMessageService = services.GetRequiredService<UpdatableMessageService>();
    }

    protected override Task StartLoading() {
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
    
    private void ProcessUpdatableAsync(GetEntities getEntities) {
        if (UpdatableMessage is null) throw new Exception("Updatable message is not set on processing");

        var subscription = _events.Watch(async e => {
            await UpdatableMessage.Refresh();
        }, getEntities);

        UpdatableMessage.OnDead += () => {
            subscription.Dispose();
        };
    }
    
    private static Func<string?> StaticHeader(string header) => () => header;
    private static Func<string?> StaticContent(string content) => () => content;
    private static Func<string?> StandardHeader() => () => null;
    
    public async Task RespondAsync(
        [Variant(nameof(StandardHeader))]
        [Variant(nameof(StaticHeader))]
        Func<string?> getHeader,
        [Variant(nameof(StaticContent))]
        Func<string?> getContent,
        GetEntities? entities = null
    ) {
        entities ??= () => [];

        await RespondAsync(() => [
            Builder<BasicComponentsBuilder>().Info(getHeader(), getContent())
        ], entities);
    }
    
    private static GetContent GetContentSingleSync(Func<IMessageComponentProperties?> getContentSingle)
        => () => [getContentSingle()];
    private static GetContentAsync GetContentSingleAsync(Func<Task<IMessageComponentProperties?>> getContentSingleAsync)
        => async () => [await getContentSingleAsync()];
    private static GetContentAsync GetContentSync(GetContent getContentSync)
        => () => Task.FromResult(getContentSync());
    
    public async Task<UpdatableMessage> RespondAsync(
        [Variant(nameof(GetContentSingleSync))]
        [Variant(nameof(GetContentSingleAsync))]
        [Variant(nameof(GetContentSync))]
        GetContentAsync getContentAsync,
        GetEntities? entities = null
    ) {
        entities ??= () => [];
        
        var needsRefresh = HasResponded;
        if (!needsRefresh) {
            await RespondComponentAsync((await getContentAsync()).OfType<IMessageComponentProperties>());
        }

        UpdatableMessage = _updatableMessageService.Watch(new UpdatableMessage(this, NetCordConfig.Root.Commands.UpdatableCommandsLifetime,
            async message => {
                await Interaction.ModifyResponseAsync(properties => properties.Components = getContentAsync().Result.OfType<IMessageComponentProperties>());
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
    
    private static GetPageContentAsync GetPageContentSync(GetPageContent getPageContentSync)
        => page => Task.FromResult(getPageContentSync(page));

    public async Task<UpdatablePageMessage> RespondPageAsync(
        [Variant(nameof(GetPageContentSync))]
        GetPageContentAsync getPageContentAsync,
        GetEntities entities
    ) {
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

    private static GetContentForOneAsync<TType> GetContentForOneSync<TType>(GetContentForOne<TType> getContentForOne)
        => item => Task.FromResult(getContentForOne(item));
    private static GetPageContentAsync GetContentForManySync(GetPageContent getContentForMany)
        => page => Task.FromResult(getContentForMany(page));
    private GetPageContent ManyItemsTitle<TType>(string manyItemsTitle, List<TType> items) {
        return page => Builder<BasicComponentsBuilder>().EntitiesList(
            manyItemsTitle,
            items.OfType<IPamelloEntity>(),
            page
        );
    }
    
    public async Task RespondOneOrManyAsync<TType>(
        List<TType> items,
        [Variant(nameof(GetContentForOneSync))]
        GetContentForOneAsync<TType> getContentForOneAsync,
        [Variant(nameof(ManyItemsTitle))]
        [Variant(nameof(GetContentForManySync))]
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

    public override string GetCallSiteInteractionDifferentiator() {
        return $"{Interaction.Id}-{CommandInteractionIndex}";
    }
}
