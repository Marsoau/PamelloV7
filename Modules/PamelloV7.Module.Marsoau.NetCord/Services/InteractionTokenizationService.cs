using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Actions;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Tokenized;

namespace PamelloV7.Module.Marsoau.NetCord.Services;


public class InteractionTokenizationService : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly DiscordButtonsService _buttons;
    private readonly DiscordModalsService _modals;
    
    private ConcurrentDictionary<string, TokenizedInteraction> Interactions { get; } = [];

    public InteractionTokenizationService(IServiceProvider services) {
        _services = services;
        
        _buttons = services.GetRequiredService<DiscordButtonsService>();
        _modals = services.GetRequiredService<DiscordModalsService>();
    }

    public TokenizedInteraction GetRequired(ButtonInteraction buttonInteraction)
        => Get(buttonInteraction) ?? throw new PamelloException($"Interaction not found by token custom id {buttonInteraction.Data.CustomId}");
    public TokenizedInteraction? Get(ButtonInteraction buttonInteraction) {
        return Interactions.GetValueOrDefault(buttonInteraction.Data.CustomId);
    }
    
    public TokenizedInteraction GetRequired(ModalInteraction modalInteraction)
        => Get(modalInteraction) ?? throw new PamelloException($"Interaction not found by token custom id {modalInteraction.Data.CustomId}");
    public TokenizedInteraction? Get(ModalInteraction modalInteraction) {
        return Interactions.GetValueOrDefault(modalInteraction.Data.CustomId);
    }

    public ButtonProperties ActionButton(string callSite, string label, ButtonStyle style, Func<Interaction, Task> action) {
        var tokenizedInteraction = new TokenizedInteraction(callSite, action);
        Interactions[tokenizedInteraction.CustomId] = tokenizedInteraction;
        
        return new ButtonProperties(tokenizedInteraction.CustomId, label, style);
    }
    
    public ButtonProperties ModalButton<TModal>(
        string callSite,
        string label,
        ButtonStyle style,
        object?[]? args = null
    )
        where TModal : DiscordModal
    {
        return ModalButton(callSite, typeof(TModal), label, style,
            async builder => await PamelloBasicActions.RunMethodAsync("Build", builder, args),
            async modal => await PamelloBasicActions.RunMethodAsync("Submit", modal, args)
        );
    }

    public ButtonProperties ModalButton<TModal>(
        string callSite,
        string label,
        ButtonStyle style,
        Func<TModal, Task> submitModal
    )
        where TModal : DiscordModal
    {
        return ModalButton(
            callSite,
            typeof(TModal),
            label,
            style,
            async builder => await PamelloBasicActions.RunMethodAsync("Build", builder),
            async m => {
                if (m is not TModal modal)
                    throw new PamelloException($"Modal {m.GetType().FullName} is not of type {typeof(TModal).FullName}");
                
                await submitModal(modal);
            }
        );
    }
    
    public ButtonProperties ModalButton<TModal, TModalBuilder>(
        string callSite,
        string label,
        ButtonStyle style,
        Func<TModalBuilder, Task> buildModal,
        Func<TModal, Task> submitModal
    )
        where TModal : DiscordModal
        where TModalBuilder : DiscordModalBuilder
    {
        return ModalButton(
            callSite,
            typeof(TModal),
            label,
            style,
            async b => {
                if (b is not TModalBuilder builder)
                    throw new PamelloException($"Modal builder {b.GetType().FullName} is not of type {typeof(TModalBuilder).FullName}");
                
                await buildModal(builder);
            },
            async m => {
                if (m is not TModal modal)
                    throw new PamelloException($"Modal {m.GetType().FullName} is not of type {typeof(TModal).FullName}");
                
                await submitModal(modal);
            }
        );
    }

    public ButtonProperties ModalButton(
        string callSite,
        Type modalType,
        string label,
        ButtonStyle style,
        Func<DiscordModalBuilder, Task> buildModal,
        Func<DiscordModal, Task> submitModal
    ) {
        var tokenizedInteraction = new TokenizedModalInteraction(
            callSite,
            async interaction => await _modals.GetBuilder(DiscordModalBuilder.GetFromModal(modalType), interaction),
            async interaction => await _modals.GetModal(modalType, interaction),
            async builder => await buildModal(builder),
            async modal => await submitModal(modal)
        );
        Interactions[tokenizedInteraction.CustomId] = tokenizedInteraction;
        
        return new ButtonProperties(tokenizedInteraction.CustomId, label, style);
    }

    public ButtonProperties Button<TButton>(string callSite, Func<TButton, Task> execute)
        where TButton : DiscordButton
    {
        var tokenizedInteraction = new TokenizedButtonInteraction<TButton>(
            callSite,
            async interaction => await _buttons.GetAsync<TButton>(interaction),
            execute
        );
        Interactions[tokenizedInteraction.CustomId] = tokenizedInteraction;

        return DiscordButtonsService.GetProperties<TButton>(tokenizedInteraction.CustomId);
    }
}
