using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Attributes;
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

public abstract partial class DiscordBasicActions
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
    
    private static Action<Interaction> OnActionSync(Action onAction)
        => _ => onAction();
    private static Func<Interaction, Task> OnInteractionSync(Action<Interaction> onInteraction)
        => interaction => {
            onInteraction(interaction);
            return Task.CompletedTask;
        };
    private static Func<Interaction, Task> OnActionAsync(Func<Task> onActionAsync)
        => _ => onActionAsync();
    
    public ButtonProperties Button(
        [Variant(nameof(GetCallSite))]
        string callSite,
        string label,
        ButtonStyle style,
        [Variant(nameof(OnInteractionSync))]
        [Variant(nameof(OnActionAsync))]
        [Variant(nameof(OnActionSync))]
        Func<Interaction, Task> onInteractionAsync
    ) => Tokenizer.ActionButton(callSite, label, style, onInteractionAsync);

    private static Action<TButton> NoExecute<TButton>() => _ => { };
    private static Func<TButton, Task> ExecuteSync<TButton>(Action<TButton> execute)
        => button => {
            execute(button);
            return Task.CompletedTask;
        };
    
    public ButtonProperties Button<TButton>(
        [Variant(nameof(GetCallSite))]
        string callSite,
        [Variant(nameof(NoExecute))]
        [Variant(nameof(ExecuteSync))]
        Func<TButton, Task> executeAsync
    )
        where TButton : DiscordButton
        => Tokenizer.Button(callSite, executeAsync);
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetCallSite() {
        var frame = new StackFrame(2, false);

        var callerMethod = frame.GetMethod();
        var ilOffset = frame.GetILOffset();

        var hash = (uint)HashCode.Combine(
            callerMethod?.DeclaringType?.FullName ?? "NoType",
            callerMethod?.Name ?? "NoMethod"
        );
        
        return $"{hash}_{ilOffset}";
    }
    
    public ButtonProperties ModalButton<TModal>(
        string label, 
        ButtonStyle style, 
        object?[]? args = null
    )
        where TModal : DiscordModal
        => Tokenizer.ModalButton<TModal>(GetCallSite(), label, style, args);
    
    public ButtonProperties ModalButton<TModal>(
        string label, 
        ButtonStyle style, 
        Func<TModal, Task> submitModal
    )
        where TModal : DiscordModal
        => Tokenizer.ModalButton<TModal>(GetCallSite(), label, style, submitModal);

    public ButtonProperties ModalButton<TModal, TModalBuilder>(
        string label, 
        ButtonStyle style, 
        Func<TModalBuilder, Task> buildModal, 
        Func<TModal, Task> submitModal
    )
        where TModal : DiscordModal
        where TModalBuilder : DiscordModalBuilder
        => Tokenizer.ModalButton<TModal, TModalBuilder>(GetCallSite(), label, style, buildModal, submitModal);

    public ButtonProperties ModalButton(
        Type modalType, 
        string label, 
        ButtonStyle style, 
        Func<DiscordModalBuilder, Task> buildModal, 
        Func<DiscordModal, Task> submitModal
    ) => Tokenizer.ModalButton(GetCallSite(), modalType, label, style, buildModal, submitModal);
    
    public TBuilder Builder<TBuilder>()
        where TBuilder : DiscordComponentBuilder
        => ComponentBuilders.Get<TBuilder>(ScopeUser);
}
