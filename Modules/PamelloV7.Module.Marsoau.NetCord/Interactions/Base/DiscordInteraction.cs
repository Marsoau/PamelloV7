using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
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

    public InteractionTokenizationService Tokenizer = null!;
    
    public bool HasResponded { get; protected set; }
    
    public virtual void Initialize(IServiceProvider services, TInteraction interaction, IPamelloUser scopeUser) {
        Interaction = interaction;
        ScopeUser = scopeUser;
        
        Services = services;
        
        ComponentBuilders = services.GetRequiredService<DiscordComponentBuilderService>();
        Commands = services.GetRequiredService<IPamelloCommandsService>();
        PEQL = services.GetRequiredService<IEntityQueryService>();
        
        Tokenizer = services.GetRequiredService<InteractionTokenizationService>();
    }
    
    public TCommand Command<TCommand>()
        where TCommand : PamelloCommand, new()
        => Commands.Get<TCommand>(ScopeUser);
    public async Task<object?> Command(string commandPath)
        => await Commands.ExecutePathAsync(commandPath, ScopeUser);
    
    public ButtonProperties Button(string label, ButtonStyle style, Action action)
        => Tokenizer.ActionButton(label, style, action);
    public ButtonProperties Button(string label, ButtonStyle style, Action<Interaction> action)
        => Tokenizer.ActionButton(label, style, action);
    public ButtonProperties Button(string label, ButtonStyle style, Func<Task> action)
        => Tokenizer.ActionButton(label, style, action);
    public ButtonProperties Button(string label, ButtonStyle style, Func<Interaction, Task> action) 
        => Tokenizer.ActionButton(label, style, action);

    public ButtonProperties Button<TButton>()
        where TButton : DiscordButton
        => Tokenizer.Button<TButton>();
    public ButtonProperties Button<TButton>(Action<TButton> execute)
        where TButton : DiscordButton
        => Tokenizer.Button(execute);
    public ButtonProperties Button<TButton>(Func<TButton, Task> execute)
        where TButton : DiscordButton
        => Tokenizer.Button(execute);
    
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
    public abstract Task ReleaseInteraction();
}
