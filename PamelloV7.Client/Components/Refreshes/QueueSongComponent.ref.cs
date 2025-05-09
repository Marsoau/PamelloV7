﻿using PamelloV7.Client.Interfaces;
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
            RefreshFavoriteState();
        }

        public void RefreshSongName() {
            Dispatcher.Invoke(() => {
                TextBlock_SongName.Text = Song?.Name;
            });
        }
        public void RefreshFavoriteState() {
            Dispatcher.Invoke(() => {
                if (_pamello.Users.Current.FavoriteSongsIds.Contains(Entry.SongId)) {
                    MenuItem_FavoriteAdd.Header = "Remove from Favorite";
                    TextBlock_FavoriteIcon.Visibility = System.Windows.Visibility.Visible;
                }
                else {
                    MenuItem_FavoriteAdd.Header = "Add to Favorite";
                    TextBlock_FavoriteIcon.Visibility = System.Windows.Visibility.Collapsed;
                }
            });
        }
    }
}
