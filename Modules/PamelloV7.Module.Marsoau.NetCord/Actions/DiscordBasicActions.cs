using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Actions;

public abstract class DiscordBasicActions
{
    public IServiceProvider Services = null!;
    
    public IPamelloUser ScopeUser = null!;
    
    public DiscordComponentBuilderService ComponentBuilders = null!;
    public IPamelloCommandsService Commands = null!;
    public IEntityQueryService PEQL = null!;

    public InteractionTokenizationService Tokenizer = null!;
    
    public IPamelloPlayer? SelectedPlayer => ScopeUser.SelectedPlayer;
    
    public virtual void InitializeActions(IServiceProvider services, IPamelloUser scopeUser) {
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
    
}
