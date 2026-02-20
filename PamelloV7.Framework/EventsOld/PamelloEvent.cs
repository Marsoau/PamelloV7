using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public abstract class PamelloEvent
    {
        public readonly EEventName EventName;

        public PamelloEvent(EEventName eventName) {
            EventName = eventName;
        }
    }
}
