using PamelloV7.Core.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.Events
{
    public class PlayerProtectionUpdated : PamelloEvent
    {
        public PlayerProtectionUpdated() : base(EEventName.PlayerProtectionUpdated) { }

        public bool IsProtected { get; set; }
    }
}
