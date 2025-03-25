using PamelloV7.Client.Pages.Refreshes;
using PamelloV7.Core.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using PamelloV7.Client.Components;
using System.Windows;

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
            RefreshCurrentSongDownloadState();
            RefreshCurrentSongDownloadProgress();
        }

        private void RefreshCurrentSongDownloadState() {
            Dispatcher.Invoke(() => {
                Console.WriteLine($"asdasdasd {_song?.IsDownloading}");
                if (_song?.IsDownloading ?? false) {
                    Grid_DownloadProgress.Visibility = Visibility.Visible;
                    Grid_SongTime.Visibility = Visibility.Collapsed;
                }
                else {
                    Grid_DownloadProgress.Visibility = Visibility.Collapsed;
                    Grid_SongTime.Visibility = Visibility.Visible;
                }
            });
        }

        public void RefreshCurrentSongDownloadProgress() {
            Dispatcher.Invoke(() => {
                ProgressBar_CurrentSong.Value = (_song?.DownloadProgress ?? 0) * 100;
            });
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
            RefreshPlayerQueueList();
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

        private void RefreshPlayerQueueList() {
            Dispatcher.Invoke(() => {
                StackPanel_Queue.Children.Clear();

                if (_player is null || _player.QueueSongsIds.Count() == 0) {
                    ScrollViewer_Queue.Visibility = System.Windows.Visibility.Collapsed;
                    TextBlock_QueueEmpty.Visibility = System.Windows.Visibility.Visible;
                    return;
                }

                foreach (var songId in _player.QueueSongsIds) {
                    StackPanel_Queue.Children.Add(new QueueSongComponent(_services) {
                        SongId = songId,
                        Margin = new System.Windows.Thickness(1),
                    });
                }

                ScrollViewer_Queue.Visibility = System.Windows.Visibility.Visible;
                TextBlock_QueueEmpty.Visibility = System.Windows.Visibility.Collapsed;
            });
        }
    }
}
