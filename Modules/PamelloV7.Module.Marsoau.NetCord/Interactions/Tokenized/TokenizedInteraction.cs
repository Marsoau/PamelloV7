using NetCord;
using PamelloV7.Module.Marsoau.NetCord.Differentiation;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Tokenized;

public class TokenizedInteraction
{
    public InteractionCallSite CallSite { get; set; }
    public Func<Interaction, Task> Action { get; set; }
    
    public TokenizedInteraction(
        InteractionCallSite callSite
    ) {
        CallSite = callSite;
        Action = _ => Task.CompletedTask;
    }
    public TokenizedInteraction(
        InteractionCallSite callSite,
        Func<Interaction, Task> action
    ) {
        CallSite = callSite;
        Action = action;
    }

    public string CustomId => CallSite.ToCustomId();
}
