using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.Services;

namespace PamelloV7.Core.Entities.Base;

public class PamelloEntitySink
{
    private readonly IServiceProvider _services;
    
    private readonly IEventsService _events;
    
    private readonly IPamelloEntity _entity;
    
    private readonly Dictionary<Type, IPamelloEvent> _savedEvents;
    
    public PamelloEntitySink(IServiceProvider services, IPamelloEntity entity) {
        _services = services;
        
        _events = services.GetRequiredService<IEventsService>();
        
        _entity = entity;
        
        _savedEvents = [];
    }

    public void Invoke<TPamelloEvent>(IPamelloUser? invoker, TPamelloEvent e)
        where TPamelloEvent : IPamelloEvent
    {
        _ = InvokeAsync(invoker, e);
    }
    public async Task<HistoryRecord?> InvokeAsync<TPamelloEvent>(IPamelloUser? invoker, TPamelloEvent e)
        where TPamelloEvent : IPamelloEvent
    {
        if (!_entity.IsChangesGoing) {
            return await _events.InvokeAsync(invoker, e);
        }
        
        if (_savedEvents.ContainsKey(e.GetType())) {
            _savedEvents[e.GetType()] = e;
        }
        else {
            _savedEvents.Add(e.GetType(), e);
        }
        
        return null;
    }

    public void Flush() {
        foreach (var (type, e) in _savedEvents) {
            _events.InvokeAsync(type, null, e); //TODO actually get a user here
        }
        
        _savedEvents.Clear();
    }
}
