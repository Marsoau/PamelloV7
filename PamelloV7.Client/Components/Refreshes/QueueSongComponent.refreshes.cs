using PamelloV7.Client.Pages.Refreshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Components
{
    public partial class QueueSongComponent : IRefrashable
    {
        public async Task Update() {
            await UpdateSong();
            await UpdateAdder();

            Refresh();
        }

        private async Task UpdateSong() {
            var song = await _pamello.Songs.Get(Entry.SongId);
            if (song is null) {
                Song = null;
                return;
            }

            Song = song;
        }
        private async Task UpdateAdder() {
            if (Entry.AdderId is null) {
                Adder = null;
                return;
            }

            var user = await _pamello.Users.Get(Entry.AdderId.Value);
            if (user is null) {
                Adder = null;
                return;
            }

            Adder = user;
        }

        public void Refresh() {
            RefreshSongName();
        }

        public void RefreshSongName() {
            TextBlock_SongName.Text = Song?.Name;
        }
    }
}
