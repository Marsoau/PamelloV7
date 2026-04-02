using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Base;

public abstract class DiscordComponentBuilder
{
    protected IServiceProvider Services = null!;
    
    protected IPamelloUser ScopeUser = null!;
    
    public IPamelloCommandsService Commands = null!;
    public InteractionTokenizationService Tokenizer = null!;
    
    public void Initialize(IServiceProvider services, IPamelloUser scopeUser) {
        ScopeUser = scopeUser;
        
        Services = services;
        
        Commands = services.GetRequiredService<IPamelloCommandsService>();
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
}
