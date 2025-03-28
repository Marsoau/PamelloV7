using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PamelloV7.Client.Interfaces;

namespace PamelloV7.Client.Components
{
    public partial class QueueSongComponent : IHasPamelloEvents
    {
        public void UnsubscribeFromEvents() {

        }

        public void SubscribeToEvents() {
            _pamello.Events.OnSongNameUpdated += Events_OnSongNameUpdated;

            _pamello.Events.OnUserFavoriteSongsUpdated += Events_OnUserFavoriteSongsUpdated;
        }

        private async Task Events_OnUserFavoriteSongsUpdated(Core.Events.UserFavoriteSongsUpdated arg) {
            if (arg.UserId == _pamello.Users.Current.Id) {
                RefreshFavoriteState();
            }
        }

        private async Task Events_OnSongNameUpdated(Core.Events.SongNameUpdated arg) {
            if (arg.SongId == Song?.Id) {
                RefreshSongName();
            }
        }
    }
}
