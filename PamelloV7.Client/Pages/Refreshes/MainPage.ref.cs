using PamelloV7.Core.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using PamelloV7.Client.Components;
using System.Windows;
using PamelloV7.Client.Interfaces;

namespace PamelloV7.Client.Pages
{
    public partial class MainPage : IRefrashable
    {
        public async Task Update() {
            await UpdatePlayer();
            await UpdateSong();

            Refresh();
        }

        private async Task UpdatePlayer() {
            if (_pamello.Users.Current?.SelectedPlayerId is null) {
                _player = null;
            }
            else {
                _player = await _pamello.Players.GetNew(_pamello.Users.Current.SelectedPlayerId.Value, true);
            }

            Console.WriteLine("player updated");
        }
        private async Task UpdateSong() {
            if (_player is null || _player.CurrentSongId is null) {
                _song = null;
            }
            else {
                _song = await _pamello.Songs.GetNew(_player.CurrentSongId.Value, true);
            }

            Console.WriteLine($"song updated to {_song?.Name ?? "none-"} ({_player?.CurrentSongId ?? -100})");
        }

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
            RefreshPlayerName();

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

        private void RefreshPlayerName() {
            Dispatcher.Invoke(() => {
                TextBlock_PlayerName.Text = _player?.Name;
            });
        }

        //Refresh Player Current Song
        private void RefreshPlayerCurrentSong() {
            RefreshPlayerCurrentSongName();
            RefreshPlayerCurrentSongCover();
            RefreshPlayerCurrentSongAdder();
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
        private void RefreshPlayerCurrentSongAdder() {
            var currentEntry = _player?.QueueEntriesDTOs.ElementAtOrDefault(_player.QueuePosition);
            Console.WriteLine($"setting adder: {currentEntry?.AdderId}, from entry: {currentEntry}");

            Dispatcher.Invoke(async () => {
                if (currentEntry is null || currentEntry.AdderId is null) {
                    TextBlock_CurrentSongAddedByLabel.Text = "Added automaticaly";
                    TextBlock_CurrentSongAddedByUser.Text = null;
                    return;
                }

                var addedBy = await _pamello.Users.Get(currentEntry.AdderId.Value);
                if (addedBy is null) return;

                TextBlock_CurrentSongAddedByLabel.Text = "Added by ";
                TextBlock_CurrentSongAddedByUser.Text = addedBy.Name;
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

                if (_player is null || _player.QueueEntriesDTOs.Count() == 0) {
                    ScrollViewer_Queue.Visibility = Visibility.Collapsed;
                    TextBlock_QueueEmpty.Visibility = Visibility.Visible;
                    return;
                }

                for (int i = 0; i < _player.QueueEntriesDTOs.Count(); i++) {
                    StackPanel_Queue.Children.Add(new QueueSongComponent(_services, i, _player.QueueEntriesDTOs.ElementAt(i)) {
                        Margin = new Thickness(1),
                        IsRequestedNext = _player.NextPositionRequest == i,
                        IsCurrent = _player.QueuePosition == i
                    });
                }

                ScrollViewer_Queue.Visibility = Visibility.Visible;
                TextBlock_QueueEmpty.Visibility = Visibility.Collapsed;
            });
        }
    }
}
