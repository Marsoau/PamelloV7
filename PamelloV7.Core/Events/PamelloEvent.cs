using PamelloV7.Core.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.Events
{
    public abstract class PamelloEvent
    {
        public readonly EEventName EventName;

        public PamelloEvent(EEventName eventName) {
            EventName = eventName;
        }
    }
}
