using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Interfaces
{
    public interface IHasPamelloEvents
    {
        public void SubscribeToEvents();
        public void UnsubscribeFromEvents();
    }
}
