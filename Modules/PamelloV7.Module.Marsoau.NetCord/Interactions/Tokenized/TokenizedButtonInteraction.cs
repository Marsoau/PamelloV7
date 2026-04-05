
using NetCord;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Tokenized;

public interface ITokenizedButtonInteraction
{
    public Task<DiscordButton> ExecuteButtonAsync(ButtonInteraction interaction);
}

public class TokenizedButtonInteraction<TButton> : TokenizedInteraction, ITokenizedButtonInteraction
    where TButton : DiscordButton
{
    private Func<ButtonInteraction, Task<TButton>> CreateButton { get; }
    private Func<TButton, Task> ExecuteButton { get; }
    
    public TokenizedButtonInteraction(
        string callSite,
        Func<ButtonInteraction, Task<TButton>> createButton,
        Func<TButton, Task> execute
    ) : base(callSite) {
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
