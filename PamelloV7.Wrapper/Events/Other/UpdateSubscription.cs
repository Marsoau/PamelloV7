using PamelloV7.Wrapper.Entities.Base;
using PamelloV7.Wrapper.Events.Base;

namespace PamelloV7.Wrapper.Events.Other;

public class UpdateSubscription
{
    public Func<IRemoteEntity?[]> WatchedEntities { get; }
    public Func<Task> Handler { get; }
    
    public UpdateSubscription(Func<Task> handler, Func<IRemoteEntity?[]> watchedEntities) {
        Handler = handler;
        WatchedEntities = watchedEntities;
    }

    public async Task InvokeAsync() {
        await Handler();
    }
}
