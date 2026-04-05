using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Config;
using PamelloV7.Module.Marsoau.Discord.Context;
using PamelloV7.Module.Marsoau.Discord.Interactions.Base;
using PamelloV7.Module.Marsoau.Discord.Messages;
using PamelloV7.Module.Marsoau.Discord.Services;
using DiscordConfig = PamelloV7.Module.Marsoau.Discord.Config.DiscordConfig;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;

public abstract class DiscordCommand : InteractionModuleBase<PamelloSocketInteractionContext>
{
    protected UpdatableMessage? _updatableMessage;
    
    protected IPamelloUser ScopeUser => Context.User;
    protected IServiceProvider Services => Context.Services;

    protected bool IsLoading { get; set; }

    protected IEntityQueryService PEQL =>
        field ??= Services.GetRequiredService<IEntityQueryService>();

    public TCommand Command<TCommand>()
        where TCommand : PamelloCommand, new()
    {
        var commands = Services.GetRequiredService<IPamelloCommandsService>();
        return commands.Get<TCommand>(Context.User);
    }
    public async Task<object?> Command(string commandPath) {
        var commands = Services.GetRequiredService<IPamelloCommandsService>();
        return await commands.ExecutePathAsync(commandPath, Context.User);
    }

    public TBuilder Builder<TBuilder>()
        where TBuilder : PamelloDiscordComponentBuilder
    {
        var builders = Services.GetRequiredService<DiscordComponentBuildersService>();
        return builders.GetBuilder<TBuilder>(Context);
    }

    protected async Task WithLoadingAsync(Task task, bool respondWithLoading = true) {
        Output.Write($"Got Result {DateTime.Now.TimeOfDay} .IsCompleted: {task.IsCompleted}");
        if (task.IsCompleted || !respondWithLoading) {
            await task;
            return;
        }
        
        Output.Write("Responding loading");
            
        await RespondLoading();
        await task;
            
        Output.Write("Wait end");

        await task;
    }
        
    protected async Task<TResult> WithLoadingAsync<TResult>(Task<TResult> task, bool respondWithLoading = true) {
        Output.Write($"Got Result {DateTime.Now.TimeOfDay} .IsCompleted: {task.IsCompleted}");
        if (!task.IsCompleted && respondWithLoading) 
        {
            Output.Write("Responding loading");
            
            await RespondLoading();
            await task;
            
            Output.Write("Wait end");
        }
    
        return await task;
    }

    public Task<TPamelloEntity> GetSingleRequiredAsync<TPamelloEntity>(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetSingleRequiredAsync<TPamelloEntity>(query, ScopeUser), respond);

    public Task<TPamelloEntity?> GetSingleAsync<TPamelloEntity>(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetSingleAsync<TPamelloEntity>(query, ScopeUser), respond);

    public Task<List<TPamelloEntity>> GetAsync<TPamelloEntity>(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetAsync<TPamelloEntity>(query, ScopeUser), respond);

    public Task<IPamelloEntity> GetSingleRequiredAsync(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetSingleRequiredAsync(query, ScopeUser), respond);

    public Task<IPamelloEntity?> GetSingleAsync(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetSingleAsync(query, ScopeUser), respond);

    public Task<List<IPamelloEntity>> GetAsync(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetAsync(query, ScopeUser), respond);

    public Task RespondInfo(string title, string description) {
        return RespondAsync(components: Builder<BasicComponentsBuilder>().Info(title, description).Build(), ephemeral: true);
    }

    public async Task ReleaseInteractionAsync() {
        if (Context.Interaction is SocketMessageComponent component) {
            var updatableMessageService = Services.GetRequiredService<UpdatableMessageKiller>();
            var updatableMessage = updatableMessageService.Get(component.Message.Id);
        
            updatableMessage?.Touch();
            
            await component.DeferAsync();
        }
    }

    public async Task RespondComponentAsync(MessageComponent component) {
        await RespondAsync(components: component, ephemeral: true);
    }
    
    public async Task RespondLoading() {
        Output.Write("Responding loading");
        if (Context.Interaction.HasResponded) {
            Output.Write("Not responding loading, message already exists");
            return;
        }

        Output.Write("Responding loading, message doesn't exist, creating");
        await RespondComponentAsync(Builder<BasicComponentsBuilder>().Defer().Build());
    }

    public Task<UpdatableMessage> RespondUpdatableAsync(Func<MessageComponent> getComponent, params IPamelloEntity[] entities) {
        return RespondUpdatableAsync(getComponent, () => entities);
    }

    private void ProcessUpdatableAsync(GetEntities getEntities) {
        if (_updatableMessage is null) throw new Exception("Updatable message is not set on processing");
        
        var events = Services.GetRequiredService<IEventsService>();

        var subscription = events.Watch(async e => {
            await _updatableMessage.Refresh();
        }, getEntities);

        _updatableMessage.OnDead += () => {
            subscription.Dispose();
        };
    }
    public async Task<UpdatableMessage> RespondUpdatableAsync(Func<MessageComponent> getComponent, GetEntities entities) {
        var needsRefresh = Context.Interaction.HasResponded;
        if (!needsRefresh) {
            await RespondComponentAsync(getComponent());
        }
        
        var updatableMessageService = Services.GetRequiredService<UpdatableMessageKiller>();

        var message = await GetOriginalResponseAsync();
        _updatableMessage = updatableMessageService.Watch(new UpdatableMessage(message, DiscordConfig.Root.Commands.UpdatableCommandsLifetime,
            async updatableMessage => {
                await ModifyOriginalResponseAsync(properties => properties.Components = getComponent());
            }, async () => {
                await DeleteOriginalResponseAsync();
            }
        ));
        
        ProcessUpdatableAsync(entities);
        
        if (needsRefresh) {
            await _updatableMessage.Refresh();
        }
        
        return _updatableMessage;
    }

    public async Task<UpdatablePageMessage> RespondUpdatablePageAsync(Func<int, MessageComponent> getPageComponent, GetEntities entities) {
        var needsRefresh = Context.Interaction.HasResponded;
        if (!needsRefresh) {
            await RespondComponentAsync(getPageComponent(0));
        }
        
        var updatableMessageService = Services.GetRequiredService<UpdatableMessageKiller>();
        
        var message = await GetOriginalResponseAsync();
        _updatableMessage = updatableMessageService.Watch(new UpdatablePageMessage(message, DiscordConfig.Root.Commands.UpdatableCommandsLifetime,
            async updatableMessage => {
                if (updatableMessage is not UpdatablePageMessage updatablePageMessage) return;
                
                await ModifyOriginalResponseAsync(p => p.Components = getPageComponent(updatablePageMessage.Page));
            }, async () => {
                await DeleteOriginalResponseAsync();
            }
        ));
        
        ProcessUpdatableAsync(entities);

        if (needsRefresh) {
            await _updatableMessage.Refresh();
        }
        
        return (UpdatablePageMessage)_updatableMessage;
    }
}
