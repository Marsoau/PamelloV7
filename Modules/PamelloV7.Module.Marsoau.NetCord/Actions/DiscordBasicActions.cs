using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Actions;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Attributes.Variants;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Other;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Actions;

public abstract partial class DiscordBasicActions : PamelloBasicActions
{
    public DiscordClientService Clients = null!;
    public DiscordComponentBuilderService ComponentBuilders = null!;
    public InteractionTokenizationService Tokenizer = null!;
    
    public override void InitializeActions(IServiceProvider services, IPamelloUser scopeUser) {
        base.InitializeActions(services, scopeUser);
        
        Clients = services.GetRequiredService<DiscordClientService>();
        ComponentBuilders = services.GetRequiredService<DiscordComponentBuilderService>();
        Tokenizer = services.GetRequiredService<InteractionTokenizationService>();
    }

    private static Func<TType, Task> ActionSync<TType>(Action<TType> action)
        => value => {
            action(value);
            return Task.CompletedTask;
        };

    [MethodImpl(MethodImplOptions.NoInlining)]
    public StringMenuProperties Select<TType>(
        [Variant(nameof(AutoCallSite))]
        [Variant(nameof(KeyedCallSite))]
        InteractionCallSite callSite,
        TType? defaultValue,
        [Variant(nameof(ActionSync))]
        Func<TType, Task> actionAsync
    ) => Tokenizer.Select(callSite, defaultValue, actionAsync);
    
    private static Action<Interaction> OnActionSync(Action onAction)
        => _ => onAction();
    private static Func<Interaction, Task> OnInteractionSync(Action<Interaction> onInteraction)
        => interaction => {
            onInteraction(interaction);
            return Task.CompletedTask;
        };
    private static Func<Interaction, Task> OnActionAsync(Func<Task> onActionAsync)
        => _ => onActionAsync();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public ButtonProperties Button(
        CommandSwitcher switcher,
        string key
    ) {
        return Button(AutoCallSite(), switcher.StateOf(key) ? "Hide" : "Show", ButtonStyle.Secondary, async () => {
            await switcher.Toggle(key);
        });
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public ButtonProperties Button(
        [Variant(nameof(AutoCallSite))]
        [Variant(nameof(KeyedCallSite))]
        InteractionCallSite callSite,
        string label,
        ButtonStyle style,
        [Variant(nameof(OnActionSync))]
        [Variant(nameof(OnActionAsync))]
        [Variant(nameof(OnInteractionSync))]
        Func<Interaction, Task> onInteractionAsync
    ) => Tokenizer.ActionButton(callSite, label, style, onInteractionAsync);

    private static Func<TButton, Task> NoExecute<TButton>()
        => button => PamelloStaticActions.ExecuteMethodAsync(button);
    private static Func<TButton, Task> ExecuteSync<TButton>(Action<TButton> execute)
        => button => {
            execute(button);
            return Task.CompletedTask;
        };
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public ButtonProperties Button<TButton>(
        [Variant(nameof(AutoCallSite), true)]
        [Variant(nameof(KeyedCallSite), true)]
        InteractionCallSite callSite,
        [Variant(nameof(NoExecute))]
        [Variant(nameof(ExecuteSync))]
        Func<TButton, Task> executeAsync
    )
        where TButton : DiscordButton
        => Tokenizer.Button(callSite, executeAsync);
    
    private static Func<DiscordModalBuilder, Task> BuildModalGeneric<TModalBuilder>(Func<TModalBuilder, Task> buildModalGeneric)
        where TModalBuilder : DiscordModalBuilder
    {
        return async b => {
            if (b is not TModalBuilder builder)
                throw new PamelloException(
                    $"Modal builder {b.GetType().FullName} is not of type {typeof(TModalBuilder).FullName}");

            await buildModalGeneric(builder);
        };
    }
    private static Func<DiscordModalBuilder, Task> BuildModalFromArgs(object?[]? args = null)
        => async builder => await PamelloStaticActions.ExecuteMethodAsync("Build", builder, args);

    private static Func<DiscordModal, Task> SubmitModalGeneric<TModal>(Func<TModal, Task> submitModalGeneric)
        where TModal : DiscordModal
    {
        return async m => {
            if (m is not TModal modal)
                throw new PamelloException(
                    $"Modal {m.GetType().FullName} is not of type {typeof(TModal).FullName}");

            await submitModalGeneric(modal);
        };
    }
    private static Func<DiscordModal, Task> SubmitModalFromArgs(object?[]? args = null)
        => async modal => await PamelloStaticActions.ExecuteMethodAsync("Submit", modal, args);
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Type ModalTypeGeneric<TModal>()
        where TModal : DiscordModal
        => typeof(TModal);
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public ButtonProperties ModalButton(
        [Variant(nameof(AutoCallSite))]
        [Variant(nameof(KeyedCallSite))]
        InteractionCallSite callSite,
        [Variant(nameof(ModalTypeGeneric))]
        Type modalType,
        string label,
        ButtonStyle style,
        [Variant(nameof(BuildModalGeneric))]
        [Variant(nameof(BuildModalFromArgs))]
        Func<DiscordModalBuilder, Task> buildModal,
        [Variant(nameof(SubmitModalGeneric))]
        [Variant(nameof(SubmitModalFromArgs))]
        Func<DiscordModal, Task> submitModal
    ) => Tokenizer.ModalButton(callSite, modalType, label, style, buildModal, submitModal);
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TBuilder Builder<TBuilder>()
        where TBuilder : DiscordComponentBuilder
        => ComponentBuilders.Get<TBuilder>(AutoCallSite(), ScopeUser);
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public InteractionCallSite AutoCallSite() => GetCallSite(-1);
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public InteractionCallSite KeyedCallSite(int key) => GetCallSite(key);
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public InteractionCallSite GetCallSite(int key) {
        var frame = new StackFrame(3, false);

        var callerMethod = frame.GetMethod();
        var ilOffset = frame.GetILOffset();

        var classHash = (uint)HashCode.Combine(
            callerMethod?.DeclaringType?.FullName ?? "NoType",
            callerMethod?.Name ?? "NoMethod"
        );

        return new InteractionCallSite(
            GetCallSiteInteractionDifferentiator(),
            classHash,
            ilOffset,
            key
        );
    }

    public abstract Differentiator GetCallSiteInteractionDifferentiator();
}
