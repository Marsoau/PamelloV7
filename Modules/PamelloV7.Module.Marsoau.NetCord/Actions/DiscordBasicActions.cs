using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Actions;

public abstract class DiscordBasicActions
{
    public IServiceProvider Services = null!;
    
    public IPamelloUser ScopeUser = null!;
    
    public DiscordClientService Clients = null!;
    
    public DiscordComponentBuilderService ComponentBuilders = null!;
    public IPamelloCommandsService Commands = null!;
    public IEntityQueryService PEQL = null!;

    public InteractionTokenizationService Tokenizer = null!;
    
    public IPamelloPlayer? SelectedPlayer => ScopeUser.SelectedPlayer;
    public IPamelloPlayer RequiredSelectedPlayer
        => SelectedPlayer ?? throw new PamelloException("Selected player required");
    public IPamelloPlayer GuaranteedSelectedPlayer
        => SelectedPlayer ?? Command<PlayerCreate>().Execute("Player");
    
    protected IPamelloQueue? Queue => SelectedPlayer?.Queue;
    protected IPamelloQueue RequiredQueue => RequiredSelectedPlayer.RequiredQueue;
    
    public virtual void InitializeActions(IServiceProvider services, IPamelloUser scopeUser) {
        ScopeUser = scopeUser;
        
        Services = services;
        
        Clients = services.GetRequiredService<DiscordClientService>();
        
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
    
    public ButtonProperties ModalButton<TModal>(
        string label, 
        ButtonStyle style
    )
        where TModal : DiscordModal
        => Tokenizer.ModalButton<TModal>(label, style);

    public ButtonProperties ModalButton<TModal>(
        string label, 
        ButtonStyle style, 
        Func<TModal, Task> submitModal
    )
        where TModal : DiscordModal
        => Tokenizer.ModalButton<TModal>(label, style, submitModal);

    public ButtonProperties ModalButton<TModal, TModalBuilder>(
        string label, 
        ButtonStyle style, 
        Func<TModalBuilder, Task> buildModal, 
        Func<TModal, Task> submitModal
    )
        where TModal : DiscordModal
        where TModalBuilder : DiscordModalBuilder
        => Tokenizer.ModalButton<TModal, TModalBuilder>(label, style, buildModal, submitModal);

    public ButtonProperties ModalButton(
        Type modalType, 
        string label, 
        ButtonStyle style, 
        Func<DiscordModalBuilder, Task> buildModal, 
        Func<DiscordModal, Task> submitModal
    ) => Tokenizer.ModalButton(modalType, label, style, buildModal, submitModal);
    
    public TBuilder Builder<TBuilder>()
        where TBuilder : DiscordComponentBuilder
        => ComponentBuilders.Get<TBuilder>(ScopeUser);
}
