using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Context;
using PamelloV7.Module.Marsoau.Discord.Interactions.Base;
using PamelloV7.Module.Marsoau.Discord.Messages;
using PamelloV7.Module.Marsoau.Discord.Services;
using DiscordConfig = PamelloV7.Module.Marsoau.Discord.Config.DiscordConfig;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;

public abstract class DiscordCommand : InteractionModuleBase<PamelloSocketInteractionContext>
{
    protected UpdatableMessage? _updatableMessage;
    
    protected IPamelloUser User => Context.User;
    protected IServiceProvider Services => Context.Services;

    protected bool IsLoading { get; set; }

    private IEntityQueryService? _peql;
    protected IEntityQueryService PEQL =>
        _peql ??= Services.GetRequiredService<IEntityQueryService>();

    public TCommand Command<TCommand>()
        where TCommand : PamelloCommand, new()
    {
        var commands = Services.GetRequiredService<IPamelloCommandsService>();
        return commands.Get<TCommand>(Context.User);
    }
    
    private async Task<TResult> WithLoadingAsync<TResult>(Task<TResult> task, bool respondWithLoading) 
    {
        Console.WriteLine($"Got Result {DateTime.Now.TimeOfDay} .IsCompleted: {task.IsCompleted}");
        if (!task.IsCompleted && respondWithLoading) 
        {
            Console.WriteLine("Responding loading");
            
            await RespondLoading();
            await task;
            
            Console.WriteLine("Wait end");
        }
    
        return await task;
    }

    public Task<TPamelloEntity> GetSingleRequiredAsync<TPamelloEntity>(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetSingleRequiredAsync<TPamelloEntity>(query, User), respond);

    public Task<TPamelloEntity?> GetSingleAsync<TPamelloEntity>(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetSingleAsync<TPamelloEntity>(query, User), respond);

    public Task<List<TPamelloEntity>> GetAsync<TPamelloEntity>(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetAsync<TPamelloEntity>(query, User), respond);

    public Task<IPamelloEntity> GetSingleRequiredAsync(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetSingleRequiredAsync(query, User), respond);

    public Task<IPamelloEntity?> GetSingleAsync(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetSingleAsync(query, User), respond);

    public Task<List<IPamelloEntity>> GetAsync(string query, bool respond = true) 
        => WithLoadingAsync(PEQL.GetAsync(query, User), respond);

    public Task RespondInfo(string title, string description) {
        return RespondAsync(components: PamelloComponentBuilders.Info(title, description).Build(), ephemeral: true);
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
        Console.WriteLine("Responding loading");
        if (Context.Interaction.HasResponded) {
            Console.WriteLine("Not responding loading, message already exists");
            return;
        }

        Console.WriteLine("Responding loading, message doesn't exist, creating");
        await RespondAsync(components: PamelloComponentBuilders.Defer().Build(), ephemeral: true);
    }

    public Task<UpdatableMessage> RespondUpdatableAsync(Action<MessageProperties> editMessage, params IPamelloEntity[] entities) {
        return RespondUpdatableAsync(editMessage, () => entities);
    }

    private async Task ProcessUpdatableAsync(Func<IPamelloEntity[]> getEntities) {
        if (_updatableMessage is null) throw new Exception("Updatable message is not set on processing");
        
        var events = Services.GetRequiredService<IEventsService>();

        var subscription = events.Watch(async e => {
            await _updatableMessage.Refresh();
        }, getEntities);

        _updatableMessage.OnDead += () => {
            subscription.Dispose();
        };
    }
    public async Task<UpdatableMessage> RespondUpdatableAsync(Action<MessageProperties> editMessage, Func<IPamelloEntity[]> entities) {
        await RespondLoading();
        
        var updatableMessageService = Services.GetRequiredService<UpdatableMessageKiller>();
        
        var message = await GetOriginalResponseAsync();
        _updatableMessage = updatableMessageService.Watch(new UpdatableMessage(message, DiscordConfig.Root.Commands.UpdatableCommandsLifetime,
            async updatableMessage => {
                await ModifyOriginalResponseAsync(editMessage);
            }, async () => {
                await DeleteOriginalResponseAsync();
            }
        ));
        
        await ProcessUpdatableAsync(entities);
        
        await _updatableMessage.Refresh();
        
        return _updatableMessage;
    }

    public async Task<UpdatablePageMessage> RespondUpdatablePageAsync(Action<MessageProperties, int> editPage, Func<IPamelloEntity[]> entities) {
        await RespondLoading();
        
        var updatableMessageService = Services.GetRequiredService<UpdatableMessageKiller>();
        
        var message = await GetOriginalResponseAsync();
        _updatableMessage = updatableMessageService.Watch(new UpdatablePageMessage(message, DiscordConfig.Root.Commands.UpdatableCommandsLifetime,
            async updatableMessage => {
                if (updatableMessage is not UpdatablePageMessage updatablePageMessage) return;
                
                await ModifyOriginalResponseAsync(p => editPage(p, updatablePageMessage.Page));
            }, async () => {
                await DeleteOriginalResponseAsync();
            }
        ));
        
        await ProcessUpdatableAsync(entities);

        await _updatableMessage.Refresh();
        
        return (UpdatablePageMessage)_updatableMessage;
    }
}
