using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Messages;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public class UpdatableMessageService : IPamelloService
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

    public UpdatableMessage? Get(ulong messageId) {
        return Messages.FirstOrDefault(message => message.Command.DiscordMessage?.Id == messageId);
    }
    
    public void KillAll() {
        var killTasks = Messages.Select(message => message.Kill());
        Task.WaitAll(killTasks.ToArray());
    }
}
