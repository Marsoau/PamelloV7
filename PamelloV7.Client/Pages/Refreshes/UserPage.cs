using PamelloV7.Client.Pages.Refreshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PamelloV7.Client.Pages
{
    public partial class UserPage : IRefrashable
    {
        public void Refresh() {
            RefreshUserName();
            RefreshAvatar();
            RefreshAddedSongsCount();
            RefreshJoinedAt();
            RefreshIds();
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
    }
}
