using PamelloV7.Core.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.Events
{
    public class PlayerCurrentAdderIdUpdated : PamelloEvent
    {
        public PlayerCurrentAdderIdUpdated() : base(EEventName.PlayerCurrentAdderIdUpdated) { }

        public int? CurrentAdderId { get; set; }
    }
}
