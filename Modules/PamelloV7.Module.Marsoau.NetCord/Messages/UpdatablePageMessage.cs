using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Messages;

public class UpdatablePageMessage : UpdatableMessage
{
    public int Page { get; private set; }
    
    public UpdatablePageMessage(
        DiscordCommand command,
        int lifetimeSeconds,
        Func<UpdatableMessage, Task<IEnumerable<IMessageComponentProperties?>>> getContent,
        Func<IEnumerable<IMessageComponentProperties?>, Task> refresh,
        Func<Task> delete
    ) : base(command, lifetimeSeconds, getContent, refresh, delete)
    { }

    public async Task SetPage(int page) {
        Page = page;
        
        await Refresh();
    }
}
