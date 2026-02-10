using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Events.Base;
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

    public TPamelloEvent Invoke<TPamelloEvent>(TPamelloEvent e)
        where TPamelloEvent : IPamelloEvent
    {
        if (!_entity.IsChangesGoing) {
            return _events.Invoke(e);
        }
        
        if (_savedEvents.ContainsKey(e.GetType())) {
            _savedEvents[e.GetType()] = e;
        }
        else {
            _savedEvents.Add(e.GetType(), e);
        }
        
        return e;
    }

    public void Flush() {
        foreach (var (type, e) in _savedEvents) {
            _events.Invoke(type, e);
        }
        
        _savedEvents.Clear();
    }
}
