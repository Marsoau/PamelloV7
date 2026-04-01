using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Messages;

public class UpdatableMessage : IDisposable
{
    public readonly DiscordCommand Command;
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
    
    public UpdatableMessage(DiscordCommand command, int lifetimeSeconds, Func<UpdatableMessage, Task> refresh, Func<Task> delete) {
        Command = command;
        
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

                Output.Write($"Added {timeLeft} seconds to lifetime");
                
                LifetimeTask = Task.Delay(timeLeft * 1000, _cancellation.Token);
            }
            
            OnDead?.Invoke();
            
            Dispose();
        });
    }

    public void Touch() {
        Output.Write("YOUCH");
        LastTouched = DateTimeOffset.Now;
    }

    public async Task Refresh() {
        if (_scheduledRefresh is not null) return;
        
        var currentTime = DateTime.Now;
        var timePassed = currentTime - _lastRefreshNew;

        if (timePassed >= _refreshIntervalNew) {
            Output.Write($"Refresh at {currentTime}");
            
            _lastRefreshNew = currentTime;
            
            await _refresh(this);
            return;
        }
        
        _ = Task.Run(async () => {
            var delaySpan = _refreshIntervalNew - timePassed;
            Output.Write($"Scheduling refresh in {delaySpan} at {currentTime}");
            _scheduledRefresh = Task.Delay(delaySpan, _cancellation.Token);
            
            await _scheduledRefresh;
            
            currentTime = DateTime.Now;
            
            Output.Write($"Awaited refresh at {currentTime}");
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
