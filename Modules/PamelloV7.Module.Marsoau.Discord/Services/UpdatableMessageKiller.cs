using PamelloV7.Core.AudioOld;
using PamelloV7.Core.Services.Base;
using PamelloV7.Module.Marsoau.Discord.Config;
using PamelloV7.Module.Marsoau.Discord.Messages;

namespace PamelloV7.Module.Marsoau.Discord.Services;

public class UpdatableMessageKiller : IPamelloService
{
    public readonly List<UpdatableMessage> Messages = [];

    public UpdatableMessage Watch(UpdatableMessage message) {
        if (Messages.Contains(message)) return message;
        
        Messages.Add(message);
        
        return message;
    }
    
    public void Remove(UpdatableMessage message) {
        Messages.Remove(message);
    }

    public UpdatableMessage? Get(ulong messageDiscordId) {
        return Messages.FirstOrDefault(message => message.DiscordMessage.Id == messageDiscordId);
    }
    
    public void KillAll() {
        var killTasks = Messages.Select(message => message.Kill());
        Task.WaitAll(killTasks.ToArray());
    }
}
