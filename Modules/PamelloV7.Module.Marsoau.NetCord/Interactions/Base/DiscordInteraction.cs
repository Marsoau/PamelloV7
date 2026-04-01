using Microsoft.Extensions.DependencyInjection;
using NetCord;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Base;

public abstract class DiscordInteraction<TInteraction>
    where TInteraction : Interaction
{
    public IServiceProvider Services = null!;
    
    public TInteraction Interaction = null!;
    public IPamelloUser ScopeUser = null!;
    
    public DiscordComponentBuilderService ComponentBuilders = null!;
    public IPamelloCommandsService Commands = null!;
    public IEntityQueryService PEQL = null!;
    
    public bool HasResponded { get; protected set; }
    
    public virtual void Initialize(IServiceProvider services, TInteraction interaction, IPamelloUser scopeUser) {
        Services = services;
        
        Interaction = interaction;
        ScopeUser = scopeUser;
        
        ComponentBuilders = services.GetRequiredService<DiscordComponentBuilderService>();
        Commands = services.GetRequiredService<IPamelloCommandsService>();
        PEQL = services.GetRequiredService<IEntityQueryService>();
    }
    
    public TCommand Command<TCommand>()
        where TCommand : PamelloCommand, new()
        => Commands.Get<TCommand>(ScopeUser);
    public async Task<object?> Command(string commandPath)
        => await Commands.ExecutePathAsync(commandPath, ScopeUser);
    
    public TBuilder Builder<TBuilder>()
        where TBuilder : DiscordComponentBuilder
        => ComponentBuilders.Get<TBuilder>(ScopeUser);
    
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

    protected async Task WithLoadingAsync(Task task, bool respondWithLoading = true) {
        if (!task.IsCompleted && respondWithLoading) {
            await RespondLoading();
        }
        
        await task;
    }
    public async Task<TResult> WithLoadingAsync<TResult>(Task<TResult> task, bool respondWithLoading = true) {
        if (!task.IsCompleted && respondWithLoading) {
            await RespondLoading();
        }
    
        return await task;
    }
    
    public abstract Task RespondLoading();
}
