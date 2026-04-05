using NetCord;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Tokenized;

public class TokenizedInteraction
{
    public string CallSite { get; set; }
    public Func<Interaction, Task> Action { get; set; }
    
    public TokenizedInteraction(
        string callSite
    ) {
        CallSite = callSite;
        Action = _ => Task.CompletedTask;
    }
    public TokenizedInteraction(
        string callSite,
        Func<Interaction, Task> action
    ) {
        CallSite = callSite;
        Action = action;
    }

    public string CustomId => $"tokenized:{CallSite}";
}
