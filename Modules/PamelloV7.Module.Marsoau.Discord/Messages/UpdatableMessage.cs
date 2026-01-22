using System.Diagnostics;
using Discord;
using PamelloV7.Core.Audio;

namespace PamelloV7.Module.Marsoau.Discord.Messages;

public class UpdatableMessage : IDisposable
{
    public readonly IUserMessage DiscordMessage;
    private readonly Func<UpdatableMessage, Task> _refresh;
    private readonly Func<Task> _delete;

    private readonly long _refreshInterval;
    private long _lastRefresh;
    
    private readonly CancellationTokenSource _cancellation;
    
    public int LifetimeSeconds { get; }
    public Task LifetimeTask { get; private set; }
    public DateTimeOffset? LastTouched { get; private set; }
    
    private Task? _scheduledRefresh;
    
    public event Action? OnDead;
    
    public UpdatableMessage(IUserMessage message, int lifetimeSeconds, Func<UpdatableMessage, Task> refresh, Func<Task> delete) {
        DiscordMessage = message;
        
        _refresh = refresh;
        _delete = delete;
        
        _refreshInterval = TimeSpan.FromSeconds(2).Ticks;
        _lastRefresh = 0;
        
        _cancellation = new CancellationTokenSource();
        
        LifetimeSeconds = lifetimeSeconds;
        LifetimeTask = Task.Delay(LifetimeSeconds * 1000, _cancellation.Token);
        LastTouched = null;
        
        _scheduledRefresh = null;
        
        StartLifetime();
    }

    private void StartLifetime() {
        Task.Run(async () => {
            while (true) {
                await LifetimeTask;
                if (LastTouched is null) break;
                
                var timePassed = DateTimeOffset.Now - LastTouched.Value;
                var timeLeft = LifetimeSeconds - (int)timePassed.TotalSeconds;
                
                if (timeLeft <= 0) break;

                Console.WriteLine($"Added {timeLeft} seconds to lifetime");
                
                LifetimeTask = Task.Delay(timeLeft * 1000, _cancellation.Token);
            }
            
            OnDead?.Invoke();
            
            Dispose();
        });
    }

    public void Touch() {
        Console.WriteLine("YOUCH");
        LastTouched = DateTimeOffset.Now;
    }

    public async Task Refresh() {
        if (_scheduledRefresh is not null) return;
        
        var currentTick = Stopwatch.GetTimestamp();

        if (currentTick - _lastRefresh >= _refreshInterval) {
            Console.WriteLine($"Refresh at {DateTime.Now.TimeOfDay}");
            
            _lastRefresh = currentTick;
            
            await _refresh(this);
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
            
            await _refresh(this);
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
