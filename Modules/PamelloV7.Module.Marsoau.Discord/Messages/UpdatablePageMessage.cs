using Discord;

namespace PamelloV7.Module.Marsoau.Discord.Messages;

public class UpdatablePageMessage : UpdatableMessage
{
    public int Page { get; private set; }
    
    public UpdatablePageMessage(IUserMessage message, int lifetimeSeconds, Func<UpdatableMessage, Task> refresh, Func<Task> delete)
        : base(message, lifetimeSeconds, refresh, delete)
    { }

    public async Task SetPage(int page) {
        Page = page;
        
        await Refresh();
    }
}
