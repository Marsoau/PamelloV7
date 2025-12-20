using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public abstract class PamelloEvent
    {
        public readonly EEventName EventName;

        public PamelloEvent(EEventName eventName) {
            EventName = eventName;
        }
    }
}
