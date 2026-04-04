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
    
    private List<TokenizedInteraction> Interactions { get; set; } = [];

    public InteractionTokenizationService(IServiceProvider services) {
        _services = services;
        
        _buttons = services.GetRequiredService<DiscordButtonsService>();
        _modals = services.GetRequiredService<DiscordModalsService>();
    }

    public TokenizedInteraction GetRequired(ButtonInteraction buttonInteraction)
        => Get(buttonInteraction) ?? throw new PamelloException($"Interaction not found by token custom id {buttonInteraction.Data.CustomId}");
    public TokenizedInteraction? Get(ButtonInteraction buttonInteraction) {
        return Interactions.FirstOrDefault(i => i.CustomId == buttonInteraction.Data.CustomId);
    }
    
    public TokenizedInteraction GetRequired(ModalInteraction modalInteraction)
        => Get(modalInteraction) ?? throw new PamelloException($"Interaction not found by token custom id {modalInteraction.Data.CustomId}");
    public TokenizedInteraction? Get(ModalInteraction modalInteraction) {
        return Interactions.FirstOrDefault(i => i.CustomId == modalInteraction.Data.CustomId);
    }

    public ButtonProperties ActionButton(string label, ButtonStyle style, Action action)
        => ActionButton(label, style, _ => action());
    public ButtonProperties ActionButton(string label, ButtonStyle style, Action<Interaction> action)
        => ActionButton(label, style, interaction => {
            action(interaction);
            return Task.CompletedTask;
        });
    public ButtonProperties ActionButton(string label, ButtonStyle style, Func<Task> action)
        => ActionButton(label, style, _ => action());
    public ButtonProperties ActionButton(string label, ButtonStyle style, Func<Interaction, Task> action) {
        var tokenizedInteraction = new TokenizedInteraction(action);
        Interactions.Add(tokenizedInteraction);
        
        return new ButtonProperties(tokenizedInteraction.CustomId, label, style);
    }
    
    public ButtonProperties ModalButton<TModal>(
        string label,
        ButtonStyle style,
        object?[]? args = null
    )
        where TModal : DiscordModal
    {
        return ModalButton(typeof(TModal), label, style,
            async builder => await PamelloBasicActions.RunMethodAsync("Build", builder, args),
            async modal => await PamelloBasicActions.RunMethodAsync("Submit", modal, args)
        );
    }

    public ButtonProperties ModalButton<TModal>(
        string label,
        ButtonStyle style,
        Func<TModal, Task> submitModal
    )
        where TModal : DiscordModal
    {
        return ModalButton(
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
        string label,
        ButtonStyle style,
        Func<TModalBuilder, Task> buildModal,
        Func<TModal, Task> submitModal
    )
        where TModal : DiscordModal
        where TModalBuilder : DiscordModalBuilder
    {
        return ModalButton(
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
        Type modalType,
        string label,
        ButtonStyle style,
        Func<DiscordModalBuilder, Task> buildModal,
        Func<DiscordModal, Task> submitModal
    ) {
        var tokenizedInteraction = new TokenizedModalInteraction(
            async interaction => await _modals.GetBuilder(DiscordModalBuilder.GetFromModal(modalType), interaction),
            async interaction => await _modals.GetModal(modalType, interaction),
            async builder => await buildModal(builder),
            async modal => await submitModal(modal)
        );
        Interactions.Add(tokenizedInteraction);
        
        return new ButtonProperties(tokenizedInteraction.CustomId, label, style);
    }

    public ButtonProperties Button<TButton>()
        where TButton : DiscordButton
        => Button<TButton>(async button => {
            await PamelloBasicActions.RunExecuteMethodAsync(button);
        });
    public ButtonProperties Button<TButton>(Action<TButton> execute)
        where TButton : DiscordButton
        => Button<TButton>(button => {
            execute(button);
            return Task.CompletedTask;
        });
    public ButtonProperties Button<TButton>(Func<TButton, Task> execute)
        where TButton : DiscordButton
    {
        var tokenizedInteraction = new TokenizedButtonInteraction<TButton>(
            async interaction => await _buttons.GetAsync<TButton>(interaction),
            execute
        );
        
        Interactions.Add(tokenizedInteraction);

        return DiscordButtonsService.GetProperties<TButton>(tokenizedInteraction.CustomId);
    }
}
