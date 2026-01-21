using System.Diagnostics;
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

    private long _refreshInterval;
    private long _lastRefresh;
    
    private Task? _scheduledRefresh;
    
    public event Action? OnDead;
    
    public UpdatableMessage(IUserMessage message, AudioTime lifetime, Func<Task> refresh, Func<Task> delete) {
        Message = message;
        
        _refresh = refresh;
        _delete = delete;
        
        _cancellation = new CancellationTokenSource();
        Lifetime = Task.Delay(lifetime.TotalSeconds * 1000, _cancellation.Token);
        
        _refreshInterval = TimeSpan.FromSeconds(2).Ticks;
        _lastRefresh = 0;
        
        _scheduledRefresh = null;
        
        StartLifetime();
    }

    private void StartLifetime() {
        Task.Run(async () => {
            await Lifetime;
            
            OnDead?.Invoke();
            
            Dispose();
        });
    }

    public async Task Refresh() {
        if (_scheduledRefresh is not null) return;
        
        var currentTick = Stopwatch.GetTimestamp();

        if (currentTick - _lastRefresh >= _refreshInterval) {
            Console.WriteLine($"Refresh at {DateTime.Now.TimeOfDay}");
            
            _lastRefresh = currentTick;
            
            await _refresh();
            return;
        }
        Console.WriteLine($"About to shedule refresh at {DateTime.Now.TimeOfDay}");
        
        Task.Run(async () => {
            var ticksLeft = _refreshInterval - (currentTick - _lastRefresh);
            if (ticksLeft <= 0) return;

            Console.WriteLine($"Scheduling refresh in {ticksLeft} at {DateTime.Now.TimeOfDay}");
            _scheduledRefresh = Task.Delay(TimeSpan.FromTicks(ticksLeft), _cancellation.Token);
            
            await _scheduledRefresh;
            Console.WriteLine($"Scheduled at {DateTime.Now.TimeOfDay}");
            _scheduledRefresh = null;
            
            _lastRefresh = Stopwatch.GetTimestamp();
            
            await _refresh();
        });
    }
    
    public async Task Kill() {
        await _cancellation.CancelAsync();
        await _delete();
    }

    public void Dispose() {
        Kill().Wait();
        
        GC.SuppressFinalize(this);
    }
}
