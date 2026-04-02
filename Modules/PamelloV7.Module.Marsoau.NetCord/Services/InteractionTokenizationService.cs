using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public class TokenizedInteraction
{
    public Guid Token { get; set; }
    public Func<Interaction, Task> Action { get; set; }
    
    public TokenizedInteraction() {
        Token = Guid.NewGuid();
        Action = _ => Task.CompletedTask;
    }
    public TokenizedInteraction(Func<Interaction, Task> action) {
        Token = Guid.NewGuid();
        Action = action;
    }

    public string CustomId => $"tokenized:{Token}";
}

public interface ITokenizedButtonInteraction
{
    public Task<DiscordButton> ExecuteButtonAsync(ButtonInteraction interaction);
}

public class TokenizedButtonInteraction<TButton> : TokenizedInteraction, ITokenizedButtonInteraction
    where TButton : DiscordButton
{
    private Func<ButtonInteraction, Task<TButton>> CreateButton { get; }
    private Func<TButton, Task> ExecuteButton { get; }
    
    public TokenizedButtonInteraction(Func<ButtonInteraction, Task<TButton>> createButton, Func<TButton, Task> execute) {
        CreateButton = createButton;
        ExecuteButton = execute;
        Action = async interaction => {
            if (interaction is not ButtonInteraction buttonInteraction) return;
            await ExecuteButtonAsync(buttonInteraction);
        };
    }

    async Task<DiscordButton> ITokenizedButtonInteraction.ExecuteButtonAsync(ButtonInteraction interaction)
        => await ExecuteButtonAsync(interaction);
    public async Task<TButton> ExecuteButtonAsync(ButtonInteraction interaction) {
        var button = await CreateButton(interaction);
        
        await ExecuteButton(button);
        
        return button;
    }
}

public class InteractionTokenizationService : IPamelloService
{
    private readonly DiscordButtonsService _buttons;
    
    private List<TokenizedInteraction> Interactions { get; set; } = [];

    public InteractionTokenizationService(IServiceProvider services) {
        _buttons = services.GetRequiredService<DiscordButtonsService>();
    }

    public TokenizedInteraction GetRequired(ButtonInteraction buttonInteraction)
        => Get(buttonInteraction) ?? throw new PamelloException($"Interaction not found by token custom id {buttonInteraction.Data.CustomId}");
    public TokenizedInteraction? Get(ButtonInteraction buttonInteraction) {
        return Interactions.FirstOrDefault(i => i.CustomId == buttonInteraction.Data.CustomId);
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

    public ButtonProperties Button<TButton>()
        where TButton : DiscordButton
        => Button<TButton>(async button => {
            await DiscordCommandsService.ExecuteMethodAsync(button);
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
