using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.Services;

namespace PamelloV7.Core.Entities.Base;

public class PamelloEntitySink
{
    private readonly IServiceProvider _services;
    
    private readonly IEventsService _events;
    
    private readonly IPamelloDynamicEntity _entity;
    
    private readonly Dictionary<Type, KeyValuePair<IPamelloUser?, IPamelloEvent>> _savedEvents;
    
    public PamelloEntitySink(IServiceProvider services, IPamelloDynamicEntity entity) {
        _services = services;
        
        _events = services.GetRequiredService<IEventsService>();
        
        _entity = entity;
        
        _savedEvents = [];
    }

    public void Invoke<TPamelloEvent>(IPamelloUser? invoker, TPamelloEvent e)
        where TPamelloEvent : IPamelloEvent
    {
        if (!_entity.IsChangesGoing) {
            _events.Invoke(invoker, e);
        }
        
        if (_savedEvents.ContainsKey(e.GetType())) {
            _savedEvents[e.GetType()] = new KeyValuePair<IPamelloUser?, IPamelloEvent>(invoker, e);
        }
        else {
            _savedEvents.Add(e.GetType(), new KeyValuePair<IPamelloUser?, IPamelloEvent>(invoker, e));
        }
    }

    public void Flush() {
        foreach (var (type, e) in _savedEvents) {
            _events.Invoke(e.Key, e.Value);
        }
        
        _savedEvents.Clear();
    }
}
