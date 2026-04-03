using NetCord;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Tokenized;

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
