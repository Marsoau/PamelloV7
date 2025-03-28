using PamelloV7.Client.Interfaces;
using PamelloV7.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace PamelloV7.Client.Pages
{
    public partial class UserPage : IRefrashable
    {
        public async Task Update()
            => await Update(null);
        public async Task Update(int? userId) {
            var id = userId ?? _user?.Id;

            if (id is not null) _user = await _pamello.Users.GetNew(id.Value, true);

            Refresh();
        }

        public void Refresh() {
            RefreshUserName();
            RefreshAvatar();
            RefreshAddedSongsCount();
            RefreshJoinedAt();
            RefreshIds();
            RefreshTabs();
        }

        private void RefreshUserName() {
            Dispatcher.Invoke(() => {
                if (User is not null) {
                    _mainWindow.Title = $"User Info | {User.Name}";
                }
                else {
                    _mainWindow.Title = $"User Info";
                }

                TextBlock_UserName.Text = User?.Name;
            });
        }

        private void RefreshAvatar() {
            if (User is null || User.AvatarUrl is null) return;

            Dispatcher.Invoke(() => {
                Image_Avatar.Source = new BitmapImage(new Uri(User.AvatarUrl));
            });
        }

        private void RefreshIds() {
            Dispatcher.Invoke(() => {
                if (User is not null) {
                    TextBlock_Ids.Text = $"{User.Id} | {User.DiscordId}";
                }
                else {
                    TextBlock_Ids.Text = $"None";
                }
            });
        }

        private void RefreshAddedSongsCount() {
            Dispatcher.Invoke(() => {
                if (User is not null) {
                    TextBlock_SongAddedCount.Text = User.AddedSongsIds.Count().ToString();
                }
                else {
                    TextBlock_SongAddedCount.Text = "None";
                }
            });
        }

        private void RefreshJoinedAt() {
            Dispatcher.Invoke(() => {
                if (User is not null) {
                    TextBlock_JoinedAt.Text = "Coming Soon";
                }
                else {
                    TextBlock_JoinedAt.Text = "Comng Soon";
                }
            });
        }

        private void RefreshTabs() {
            RefreshFavoriteSongsTabHeader();
            RefreshFavoriteSongsList();
        }

        private void RefreshFavoriteSongsTabHeader() {
            Dispatcher.Invoke(() => {
                TabItem_FavoriteSongs.Header = $"Favorite Songs ({User?.FavoriteSongsIds.Count() ?? 0})";
            });
        }

        private void RefreshFavoriteSongsList() {
            Dispatcher.Invoke(() => {
                StackPanel_FavoriteSongs.Children.Clear();

                if (User is null) return;

                foreach (var songId in User.FavoriteSongsIds) {
                    StackPanel_FavoriteSongs.Children.Add(new SongComponent(_services, songId) {
                        Margin = new Thickness(2)
                    });
                }
            });
        }
    }
}
