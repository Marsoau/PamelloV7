using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.Actions;

public abstract class PamelloBasicActions
{
    private bool _isInitialized;
    
    public IServiceProvider Services = null!;
    
    public IPamelloUser ScopeUser = null!;
    
    public IEntityQueryService PEQL = null!;
    public IPamelloCommandsService Commands = null!;
    
    public IPamelloPlayer? SelectedPlayer => ScopeUser.SelectedPlayer;
    public IPamelloPlayer RequiredSelectedPlayer
        => SelectedPlayer ?? throw new PamelloException("Selected player required");
    public IPamelloPlayer GuaranteedSelectedPlayer
        => SelectedPlayer ?? Command<PlayerCreate>().Execute("Player");
    
    protected IPamelloQueue? Queue => SelectedPlayer?.Queue;
    protected IPamelloQueue RequiredQueue => RequiredSelectedPlayer.RequiredQueue;
    
    public bool RespondedWithLoading { get; protected set; }
    
    public virtual void InitializeActions(IServiceProvider services, IPamelloUser scopeUser) {
        if (_isInitialized) return;
        _isInitialized = true;
        
        Services = services;
        
        ScopeUser = scopeUser;
        
        PEQL = services.GetRequiredService<IEntityQueryService>();
        Commands = services.GetRequiredService<IPamelloCommandsService>();
    }
    
    public TCommand Command<TCommand>()
        where TCommand : PamelloCommand, new()
        => Commands.Get<TCommand>(ScopeUser);
    public async Task<object?> Command(string commandPath)
        => await Commands.ExecutePathAsync(commandPath, ScopeUser);
    
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
    
    public async Task WithLoadingAsync(Task task, bool respondWithLoading = true) {
        if (!task.IsCompleted && !RespondedWithLoading && respondWithLoading) {
            await StartLoading();
        }
        
        await task;
    }
    public async Task<TResult> WithLoadingAsync<TResult>(Task<TResult> task, bool respondWithLoading = true) {
        if (!task.IsCompleted && !RespondedWithLoading && respondWithLoading) {
            await StartLoading();
        }
    
        return await task;
    }

    protected virtual Task StartLoading() {
        return Task.CompletedTask;
    }
}
