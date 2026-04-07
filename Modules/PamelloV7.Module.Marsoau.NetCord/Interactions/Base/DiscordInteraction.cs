using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Actions;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Base;

public abstract class DiscordInteraction<TInteraction> : DiscordBasicActions
    where TInteraction : Interaction
{
    public TInteraction Interaction => _interaction as TInteraction
        ?? throw new InvalidOperationException($"Interaction is not of type {typeof(TInteraction).FullName}");
    
    public bool HasResponded { get; protected set; }
    
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

    public Task RespondLoading() {
        if (HasResponded) return Task.CompletedTask;
        
        var task = RespondLoadingInternal();
        
        HasResponded = true;
        
        return task;
    }
    public Task ReleaseInteraction() {
        if (HasResponded) return Task.CompletedTask;
        
        var task = ReleaseInteractionInternal();
        
        HasResponded = true;
        
        return task;
    }
    
    protected abstract Task RespondLoadingInternal();
    protected abstract Task ReleaseInteractionInternal();
}
