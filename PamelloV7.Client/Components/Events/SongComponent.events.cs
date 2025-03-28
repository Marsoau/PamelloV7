using PamelloV7.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Components
{
    public partial class SongComponent : IHasPamelloEvents
    {
        public void UnsubscribeFromEvents() => throw new NotImplementedException();

        public void SubscribeToEvents() {
            _pamello.Events.OnUserFavoriteSongsUpdated += Events_OnUserFavoriteSongsUpdated;
        }

        private async Task Events_OnUserFavoriteSongsUpdated(Core.Events.UserFavoriteSongsUpdated arg) {
            RefreshFavoriteState();
        }
    }
}
