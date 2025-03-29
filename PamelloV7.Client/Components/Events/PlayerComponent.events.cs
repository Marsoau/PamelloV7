using PamelloV7.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Components
{
    public partial class PlayerComponent : IHasPamelloEvents
    {
        public void UnsubscribeFromEvents() => throw new NotImplementedException();
        public void SubscribeToEvents() {
            _pamello.Events.OnPlayerNameUpdated += Events_OnPlayerNameUpdated;
        }

        private async Task Events_OnPlayerNameUpdated(Core.Events.PlayerNameUpdated arg) {
            RefreshPlayerName();
        }
    }
}
