using PamelloV7.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Pages
{
    public partial class PlayersPage : IHasPamelloEvents
    {
        public void UnsubscribeFromEvents() => throw new NotImplementedException();
        public void SubscribeToEvents() {
            _pamello.Events.OnUserSelectedPlayerIdUpdated += Events_OnUserSelectedPlayerIdUpdated;
            _pamello.Events.OnPlayerAvailable += Events_OnPlayerAvailable;

            _pamello.Events.OnPlayerNameUpdated += Events_OnPlayerNameUpdated;
            _pamello.Events.OnPlayerProtectionUpdated += Events_OnPlayerProtectionUpdated;
        }

        private async Task Events_OnPlayerProtectionUpdated(Core.Events.PlayerProtectionUpdated arg) {
            RefreshOptionsPlayerProtection();
        }

        private async Task Events_OnPlayerNameUpdated(Core.Events.PlayerNameUpdated arg) {
            RefreshOptionsPlayerName();
        }

        private async Task Events_OnPlayerAvailable(Core.Events.PlayerAvailable arg) {
            RefreshPlayers();
        }

        private async Task Events_OnUserSelectedPlayerIdUpdated(Core.Events.UserSelectedPlayerIdUpdated arg) {
            await Update();
        }
        
    }
}
