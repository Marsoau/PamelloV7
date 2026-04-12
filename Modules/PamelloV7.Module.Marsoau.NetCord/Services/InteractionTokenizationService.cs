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
using PamelloV7.Module.Marsoau.NetCord.Differentiation;
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
    
    private readonly ConcurrentDictionary<string, TokenizedInteraction> _interactions = [];

    public InteractionTokenizationService(IServiceProvider services) {
        _services = services;
        
        _buttons = services.GetRequiredService<DiscordButtonsService>();
        _modals = services.GetRequiredService<DiscordModalsService>();
    }

    public TokenizedInteraction GetRequired(string customId)
        => Get(customId) ?? throw new PamelloException($"Interaction with custom id \"{customId}\" not found");
    
    public TokenizedInteraction? Get(string customId)
        => _interactions.GetValueOrDefault(customId);

    public ButtonProperties ActionButton(InteractionCallSite callSite, string label, ButtonStyle style, Func<Interaction, Task> action) {
        var tokenizedInteraction = new TokenizedInteraction(callSite, action);
        _interactions[tokenizedInteraction.CustomId] = tokenizedInteraction;
        
        return new ButtonProperties(tokenizedInteraction.CustomId, label, style);
    }
    
    public StringMenuProperties Select<TType>(InteractionCallSite callSite, TType? defaultValue, Func<TType, Task> action) {
        var tokenizedInteraction = new TokenizedSelectInteraction<TType>(callSite, action, _services);
        _interactions[tokenizedInteraction.CustomId] = tokenizedInteraction;
        
        var properties = new StringMenuProperties(tokenizedInteraction.CustomId);
        
        if (!typeof(TType).IsAssignableTo(typeof(Enum))) return properties;
        
        var values = Enum.GetValues(typeof(TType)).Cast<Enum>();
        foreach (var value in values) {
            if (value is null) continue;
            
            properties.AddOptions(new StringMenuSelectOptionProperties(value.ToString(), value.ToString("D")).WithDefault(Equals(value, defaultValue as Enum)));
        }
        
        return properties;
    }

    public ButtonProperties ModalButton(
        InteractionCallSite callSite,
        Type modalType,
        string label,
        ButtonStyle style,
        Func<DiscordModalBuilder, Task> buildModal,
        Func<DiscordModal, Task> submitModal
    ) {
        var tokenizedInteraction = new TokenizedModalInteraction(
            callSite,
            async interaction => await _modals.GetBuilder(DiscordModalBuilder.GetTypeFromModal(modalType), interaction),
            async interaction => await _modals.GetModal(modalType, interaction),
            async builder => await buildModal(builder),
            async modal => await submitModal(modal)
        );
        _interactions[tokenizedInteraction.CustomId] = tokenizedInteraction;
        
        return new ButtonProperties(tokenizedInteraction.CustomId, label, style);
    }

    public ButtonProperties Button<TButton>(InteractionCallSite callSite, Func<TButton, Task> execute)
        where TButton : DiscordButton
    {
        var tokenizedInteraction = new TokenizedButtonInteraction<TButton>(
            callSite,
            async interaction => await _buttons.GetAsync<TButton>(interaction),
            execute
        );
        _interactions[tokenizedInteraction.CustomId] = tokenizedInteraction;

        return DiscordButtonsService.GetProperties<TButton>(tokenizedInteraction.CustomId);
    }
}
