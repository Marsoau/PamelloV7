using NetCord.Rest;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Messages;

public class UpdatableMessage : IDisposable
{
    public readonly DiscordCommand Command;
    
    private readonly Func<UpdatableMessage, Task<IEnumerable<IMessageComponentProperties?>>> _getContent;
    private readonly Func<IEnumerable<IMessageComponentProperties?>, Task> _refresh;
    private readonly Func<Task> _delete;

    private readonly TimeSpan _refreshInterval;
    private DateTime _lastRefresh;
    private DateTime _firstAppearance;
    
    private readonly CancellationTokenSource _cancellation;

    public static int MaximumLifetimeSeconds { get; } = 10 * 60;
    public int LifetimeSeconds { get; }
    public Task LifetimeTask { get; private set; }
    
    public DateTimeOffset? LastTouched { get; private set; }

    public DateTime LastRefreshInGroup
        => Command.PartCommands.Select(c => c.UpdatableMessage?._lastRefresh).OfType<DateTime>().Max();

    private Task? _scheduledRefresh;
    
    public event Action? OnDead;
    
    public UpdatableMessage(
        DiscordCommand command,
        int lifetimeSeconds,
        Func<UpdatableMessage, Task<IEnumerable<IMessageComponentProperties?>>> getContent,
        Func<IEnumerable<IMessageComponentProperties?>, Task> refresh,
        Func<Task> delete
    ) {
        Command = command;

        _getContent = getContent;
        _refresh = refresh;
        _delete = delete;
        
        _refreshInterval = TimeSpan.FromSeconds(1);
        
        _lastRefresh = DateTime.MinValue;
        _firstAppearance = DateTime.Now;
        
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
                
                var totalTimePassed = DateTimeOffset.Now - _firstAppearance;
                var maxTimeLeft = MaximumLifetimeSeconds - (int)totalTimePassed.TotalSeconds;
                
                if (maxTimeLeft < timeLeft) timeLeft = maxTimeLeft;
                
                if (timeLeft <= 0) break;

                Output.Write($"Added {timeLeft} seconds to lifetime");
                
                LifetimeTask = Task.Delay(timeLeft * 1000, _cancellation.Token);
            }
            
            OnDead?.Invoke();
            
            Dispose();
        });
    }

    public void Touch() {
        LastTouched = DateTimeOffset.Now;
        
        for (var i = 0; i < Command.PartIndex; i++) {
            Command.PartCommands[i].UpdatableMessage?.Touch();
        }
    }

    public async Task<IEnumerable<IMessageComponentProperties?>> GetContent(bool all = true) {
        if (!all) return await _getContent(this);
        
        var content = new List<IMessageComponentProperties?>();
        
        foreach (var part in Command.PartCommands) {
            if (part.UpdatableMessage is null) continue;
            
            content.AddRange(await part.UpdatableMessage.GetContent(false));
        }
        
        return content;
    }

    public async Task Refresh() {
        if (_scheduledRefresh is not null) return;
        
        var currentTime = DateTime.Now;
        var timePassed = currentTime - LastRefreshInGroup;

        if (timePassed >= _refreshInterval) {
            //Output.Write($"Refresh at {currentTime}");
            
            _lastRefresh = currentTime;
            
            await _refresh(await GetContent());
            return;
        }
        
        _ = Task.Run(async () => {
            var delaySpan = _refreshInterval - timePassed;
            //Output.Write($"Scheduling refresh in {delaySpan} at {currentTime}");
            _scheduledRefresh = Task.Delay(delaySpan, _cancellation.Token);
            
            await _scheduledRefresh;
            
            currentTime = DateTime.Now;
            
            //Output.Write($"Awaited refresh at {currentTime}");
            _scheduledRefresh = null;

            _lastRefresh = currentTime;
            
            await _refresh(await GetContent());
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
