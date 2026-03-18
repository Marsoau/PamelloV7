using System.Diagnostics;
using Discord;

namespace PamelloV7.Module.Marsoau.Discord.Messages;

public class UpdatableMessage : IDisposable
{
    public readonly IUserMessage DiscordMessage;
    private readonly Func<UpdatableMessage, Task> _refresh;
    private readonly Func<Task> _delete;

    private readonly TimeSpan _refreshIntervalNew;
    private DateTime _lastRefreshNew;
    
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
        
        _refreshIntervalNew = TimeSpan.FromSeconds(1);
        _lastRefreshNew = DateTime.Now;
        
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
        
        var currentTime = DateTime.Now;
        var timePassed = currentTime - _lastRefreshNew;

        if (timePassed >= _refreshIntervalNew) {
            Console.WriteLine($"Refresh at {currentTime}");
            
            _lastRefreshNew = currentTime;
            
            await _refresh(this);
            return;
        }
        
        Task.Run(async () => {
            var delaySpan = _refreshIntervalNew - timePassed;
            Console.WriteLine($"Scheduling refresh in {delaySpan} at {currentTime}");
            _scheduledRefresh = Task.Delay(delaySpan, _cancellation.Token);
            
            await _scheduledRefresh;
            
            currentTime = DateTime.Now;
            
            Console.WriteLine($"Awaited refresh at {currentTime}");
            _scheduledRefresh = null;

            _lastRefreshNew = currentTime;
            
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
