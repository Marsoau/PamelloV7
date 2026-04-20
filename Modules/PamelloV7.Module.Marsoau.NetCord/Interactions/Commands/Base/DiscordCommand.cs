using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Actions;
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
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;
using PamelloV7.Module.Marsoau.NetCord.Messages;
using PamelloV7.Module.Marsoau.NetCord.Services;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

public abstract partial class DiscordCommand : DiscordInteraction<SlashCommandInteraction>, IAsyncDisposable
{
    public delegate IEnumerable<IMessageComponentProperties?> GetContent();
    public delegate IEnumerable<IMessageComponentProperties?> GetContentForOne<TType>(TType item);
    public delegate IEnumerable<IMessageComponentProperties?> GetPageContent(int page);
    
    public delegate Task<IEnumerable<IMessageComponentProperties?>> GetContentAsync();
    public delegate Task<IEnumerable<IMessageComponentProperties?>> GetContentForOneAsync<TType>(TType item);
    public delegate Task<IEnumerable<IMessageComponentProperties?>> GetPageContentAsync(int page);
    
    private IEventsService _events = null!;
    private UpdatableMessageService _updatableMessageService = null!;
    
    private DiscordCommandsService _discordCommands = null!;
    
    public UpdatableMessage? UpdatableMessage { get; private set; }
    public RestMessage? DiscordMessage { get; private set; }
    
    public DiscordCommand? ParentFollowUp { get; set; }
    public List<DiscordCommand> FollowUpCommands = [];
    public int FollowUpIndex;
    
    public DiscordCommand? ParentPart { get; set; }
    public List<DiscordCommand> PartCommands = [];
    public int PartIndex;

    public virtual void InitializeCommand(
        Interaction interaction,
        DiscordCommand? parentCommand,
        bool isFollowUp,
        IServiceProvider services,
        IPamelloUser scopeUser
    ) {
        InitializeInteraction(interaction, services, scopeUser);

        if (isFollowUp) {
            ParentFollowUp = parentCommand;
        }
        else {
            ParentPart = parentCommand;
            DiscordMessage = ParentPart?.DiscordMessage;
        }
        
        if (ParentFollowUp is null) {
            FollowUpCommands = [this];
            
            FollowUpIndex = 0;
        }
        else {
            FollowUpCommands = ParentFollowUp.FollowUpCommands;
            FollowUpCommands.Add(this);
            
            FollowUpIndex = FollowUpCommands.Count - 1;
        }
        
        if (ParentPart is null) {
            PartCommands = [this];
            
            PartIndex = 0;
        }
        else {
            PartCommands = ParentPart.PartCommands;
            PartCommands.Add(this);
            
            PartIndex = PartCommands.Count - 1;
        }
        
        _events = services.GetRequiredService<IEventsService>();
        _updatableMessageService = services.GetRequiredService<UpdatableMessageService>();
        _discordCommands = services.GetRequiredService<DiscordCommandsService>();
    }

    private static Type GenericCommandType<TCommand>() => typeof(TCommand);

    public async Task<DiscordCommand> RespondCommandAsync(
        [Variant(nameof(GenericCommandType))]
        Type commandType,
        params object?[] args
    ) {
        var command = await _discordCommands.GetAsync(commandType, false, Interaction, this);
        await PamelloStaticActions.ExecuteMethodAsync(command, args);

        return command;
    }

    public async Task<TCommand> AddCommand<TCommand>(bool isFollowUp = false)
        where TCommand : DiscordCommand
        => await _discordCommands.GetAsync<TCommand>(Interaction, this, isFollowUp);

    protected override Task StartLoading() {
        return RespondComponentAsync([
            BasicComponentsBuilder.Loading()
        ]);
    }

    public async Task RespondComponentAsync(IEnumerable<IMessageComponentProperties?> components) {
        var messageComponents = components.OfType<IMessageComponentProperties>();
        
        if (DiscordMessage is not null) {
            if (FollowUpIndex != 0) {
                await Interaction.ModifyFollowupMessageAsync(DiscordMessage.Id, properties => properties.Components = messageComponents);
            }
            else {
                await Interaction.ModifyResponseAsync(properties => properties.Components = messageComponents);
            }
            
            return;
        }

        var messageProperties = new InteractionMessageProperties() {
            Components = messageComponents,
            Flags = MessageFlags.Ephemeral | MessageFlags.IsComponentsV2
        };

        if (FollowUpIndex != 0) {
            DiscordMessage = await Interaction.SendFollowupMessageAsync(messageProperties);
            Output.Write($"Message after followup: {DiscordMessage?.Id}");
        }
        else {
            var messageCallback = InteractionCallback.Message(messageProperties);
            var callback = await Interaction.SendResponseAsync(messageCallback, true);
            DiscordMessage = callback?.Resource.Message;
            Output.Write($"Message after: {DiscordMessage?.Id}");
        }
    }

    public async Task DeleteResponseAsync() {
        if (DiscordMessage is null) return;
        
        if (FollowUpIndex != 0) {
            await Interaction.DeleteFollowupMessageAsync(DiscordMessage.Id);
        }
        else {
            await Interaction.DeleteResponseAsync();
        }
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

    private static (Func<string?> getContent, GetEntities getEntities) ContentForEntity(Func<IPamelloEntity?> getEntity) {
        return (
            () => getEntity()?.ToDiscordString() ?? "No entity",
            () => [getEntity()]
        );
    }
    
    public async Task RespondAsync(
        [Variant(nameof(StandardHeader))]
        [Variant(nameof(StaticHeader))]
        Func<string?> getHeader,
        [Variant(nameof(StaticContent))]
        [Variant(nameof(ContentForEntity))]
        Func<string?> getContent,
        GetEntities? entities = null
    ) {
        entities ??= () => [];

        await RespondAsync(() => [
            BasicComponentsBuilder.Info(getHeader(), getContent())
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
        GetEntities? entities = null,
        int? lifetimeSeconds = null
    ) {
        entities ??= () => [];

        UpdatableMessage = _updatableMessageService.Watch(new UpdatableMessage(this, lifetimeSeconds ?? NetCordConfig.Root.Commands.UpdatableCommandsLifetime,
            _ => getContentAsync(),
            async components => {
                await RespondComponentAsync(components);
            }, async () => {
                await DisposeAsync();
            }
        ));
        
        ProcessUpdatableAsync(entities);
        
        await UpdatableMessage.Refresh();
        
        return UpdatableMessage;
    }
    
    private static GetPageContentAsync GetPageContentSync(GetPageContent getPageContentSync)
        => page => Task.FromResult(getPageContentSync(page));

    public async Task<UpdatablePageMessage> RespondPageAsync(
        [Variant(nameof(GetPageContentSync))]
        GetPageContentAsync getPageContentAsync,
        GetEntities entities
    ) {
        UpdatableMessage = _updatableMessageService.Watch(new UpdatablePageMessage(this, NetCordConfig.Root.Commands.UpdatableCommandsLifetime,
            async message => {
                if (message is not UpdatablePageMessage updatablePageMessage) return [];
                
                return await getPageContentAsync(updatablePageMessage.Page);
            },
            async components => {
                await RespondComponentAsync(components);
            }, async () => {
                await DisposeAsync();
            }
        ));
        
        ProcessUpdatableAsync(entities);

        await UpdatableMessage.Refresh();
        
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

    public override Differentiator GetCallSiteInteractionDifferentiator() {
        return new Differentiator(
            Interaction.Id,
            FollowUpIndex,
            PartIndex
        );
    }

    public async ValueTask DisposeAsync() {
        if (ParentPart is not null) {
            PartCommands.RemoveAt(PartIndex);
            
            var parentMessage = ParentPart.UpdatableMessage;
            if (parentMessage is null) return;
            
            await parentMessage.Refresh();
        }
        else if (ParentFollowUp is not null) {
            FollowUpCommands.RemoveAt(FollowUpIndex);
            
            var parentFollowUpMessage = ParentFollowUp.UpdatableMessage;
            if (parentFollowUpMessage is null) return;
            
            await parentFollowUpMessage.Refresh();
        }
        else {
            await DeleteResponseAsync();
        }
    }
}
