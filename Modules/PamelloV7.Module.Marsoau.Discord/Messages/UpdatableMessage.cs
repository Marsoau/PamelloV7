using Discord;
using PamelloV7.Core.Audio;

namespace PamelloV7.Module.Marsoau.Discord.Messages;

public class UpdatableMessage : IDisposable
{
    public readonly IUserMessage Message;
    private readonly Func<Task> _refresh;
    private readonly Func<Task> _delete;
    
    public readonly Task Lifetime;
    private readonly CancellationTokenSource _cancellation;
    
    public UpdatableMessage(IUserMessage message, AudioTime lifetime, Func<Task> refresh) {
        Message = message;
        
        _refresh = refresh;
        
        _cancellation = new CancellationTokenSource();
        
        Lifetime = Task.Delay(lifetime.TotalSeconds * 1000, _cancellation.Token);
    }

    public async Task Refresh() {
        await _refresh();
    }
    
    public async Task Delete() {
        await _cancellation.CancelAsync();
        Dispose();
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
    }
}
