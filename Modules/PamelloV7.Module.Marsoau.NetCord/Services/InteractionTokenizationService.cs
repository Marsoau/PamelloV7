using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Actions;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Attributes.Variants;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Tokenized;

namespace PamelloV7.Module.Marsoau.NetCord.Services;


public partial class InteractionTokenizationService : IPamelloService
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
    public TokenizedInteraction GetRequired(ModalInteraction modalInteraction)
        => Get(modalInteraction) ?? throw new PamelloException($"Interaction not found by token custom id {modalInteraction.Data.CustomId}");
    
    private static string ModalInteractionId(ModalInteraction modalInteraction)
        => modalInteraction.Data.CustomId;
    private static string ButtonInteractionId(ButtonInteraction buttonInteraction)
        => buttonInteraction.Data.CustomId;
    
    [RequiredVariant]
    public TokenizedInteraction? Get(
        [Variant(nameof(ButtonInteractionId))]
        [Variant(nameof(ModalInteractionId))]
        string customId
    ) {
        return Interactions.GetValueOrDefault(customId);
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
