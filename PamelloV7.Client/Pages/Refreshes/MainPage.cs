using PamelloV7.Client.Pages.Refreshes;
using PamelloV7.Core.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PamelloV7.Client.Pages
{
    public partial class MainPage : IRefrashable
    {
        public void Refresh() {
            Dispatcher.Invoke(() => {
                TextBlock_Username.Text = _pamello.Users.Current.Name;
                TextBlock_Userids.Text = $"{_pamello.Users.Current.Id} | {_pamello.Users.Current.DiscordId}";

                Image_Avatar.Source = new BitmapImage(new Uri(_pamello.Users.Current.AvatarUrl ?? ""));
            });

            RefreshPlayer();
        }

        //Refresh Player
        private void RefreshPlayer() {
            RefreshPlayerCurrentSong();

            RefreshPlayerIsPaused();

            RefreshPlayerCurrentSongTimePassed();
            RefreshPlayerCurrentSongTimeTotal();

            RefreshPlayerState();

            RefreshPlayerQueue();
        }

        private void RefreshPlayerIsPaused() {
            Dispatcher.Invoke(() => {
                Button_ResumePause1.Content = (_player?.IsPaused ?? false) ? "R" : "P";
                Button_ResumePause2.Content = (_player?.IsPaused ?? false) ? "R" : "P";
            });
        }
        private void RefreshPlayerState() {
            Dispatcher.Invoke(() => {
                TextBlock_PlayerState.Text = _player?.State.ToString();
            });
        }
        private void RefreshPlayerCurrentSongTimePassed() {
            var time = new AudioTime(_player?.CurrentSongTimePassed ?? 0);
            var progress = 0.0;
            if (_player is not null) {
                progress = (double)_player.CurrentSongTimePassed / _player.CurrentSongTimeTotal;
                if (double.IsNaN(progress)) progress = 0.0;
            }

            Dispatcher.Invoke(() => {
                TextBlock_CurrentSongTimePassed.Text = time.ToShortString();
                Slider_CurrentSongTime.Value = progress;
            });
        }
        private void RefreshPlayerCurrentSongTimeTotal() {
            var time = new AudioTime(_player?.CurrentSongTimeTotal ?? 0);

            Dispatcher.Invoke(() => {
                TextBlock_CurrentSongTimeTotal.Text = time.ToShortString();
            });
        }

        //Refresh Player Current Song
        private void RefreshPlayerCurrentSong() {
            RefreshPlayerCurrentSongName();
            RefreshPlayerCurrentSongCover();
            RefreshPlayerCurrentSongAddedBy();
        }

        private void RefreshPlayerCurrentSongCover() {
            Dispatcher.Invoke(() => {
                Image_CurrentSongCover.Source = new BitmapImage(new Uri(_song?.CoverUrl ?? "https://static.vecteezy.com/system/resources/thumbnails/022/059/000/small_2x/no-image-available-icon-vector.jpg"));
            });
        }
        private void RefreshPlayerCurrentSongName() {
            Dispatcher.Invoke(() => {
                TextBlock_CurrentSongName.Text = _song?.Name;
            });
        }
        private void RefreshPlayerCurrentSongAddedBy() {
            Dispatcher.Invoke(async () => {
                if (_song is null) return;

                var addedBy = await _pamello.Users.Get(_song.AddedById);
                if (addedBy is null) return;

                TextBlock_CurrentSongAddedBy.Text = addedBy.Name;
            });
        }
        
        //Refresh Player Queue
        private void RefreshPlayerQueue() {
            RefreshPlayerQueueModes();
        }

        private void RefreshPlayerQueueModes() {
            RefreshPlayerQueueIsRandom();
            RefreshPlayerQueueIsReversed();
            RefreshPlayerQueueIsNoLeftovers();
            RefreshPlayerQueueIsFeedRandom();
        }

        private void RefreshPlayerQueueIsRandom() {
            Dispatcher.Invoke(() => {
                CheckBox_Random.IsChecked = _player?.QueueIsRandom ?? false;
            });
        }
        private void RefreshPlayerQueueIsReversed() {
            Dispatcher.Invoke(() => {
                CheckBox_Reversed.IsChecked = _player?.QueueIsReversed ?? false;
            });
        }
        private void RefreshPlayerQueueIsNoLeftovers() {
            Dispatcher.Invoke(() => {
                CheckBox_NoLeftovers.IsChecked = _player?.QueueIsNoLeftovers ?? false;
            });
        }
        private void RefreshPlayerQueueIsFeedRandom() {
            Dispatcher.Invoke(() => {
                CheckBox_FeedRandom.IsChecked = _player?.QueueIsFeedRandom ?? false;
            });
        }
    }
}
