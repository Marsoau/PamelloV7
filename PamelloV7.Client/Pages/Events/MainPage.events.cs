using PamelloV7.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Pages
{
    public partial class MainPage : IHasPamelloEvents
    {
        public void UnsubscribeFromEvents() {

        }
        public void SubscribeToEvents() {
            _pamello.Events.OnUserSelectedPlayerIdUpdated += Events_OnUserSelectedPlayerIdUpdated;

            _pamello.Events.OnPlayerQueueEntriesDTOsUpdated += Events_OnPlayerQueueSongsIdsUpdated;

            //Song
            _pamello.Events.OnPlayerCurrentSongIdUpdated += Events_OnPlayerCurrentSongIdUpdated;

            _pamello.Events.OnPlayerIsPausedUpdated += Events_OnPlayerIsPausedUpdated;
            _pamello.Events.OnSongDownloadFinished += Events_OnSongDownloadFinished;
            _pamello.Events.OnSongDownloadStarted += Events_OnSongDownloadStarted;
            _pamello.Events.OnSongDownloadProgeressUpdated += Events_OnSongDownloadProgeressUpdated;

            _pamello.Events.OnPlayerCurrentSongTimePassedUpdated += Events_OnPlayerCurrentSongTimePassedUpdated;
            _pamello.Events.OnPlayerCurrentSongTimeTotalUpdated += Events_OnPlayerCurrentSongTimeTotalUpdated; ;

            _pamello.Events.OnPlayerStateUpdated += Events_OnPlayerStateUpdated;

            _pamello.Events.OnPlayerQueueIsRandomUpdated += Events_OnPlayerQueueIsRandomUpdated;
            _pamello.Events.OnPlayerQueueIsReversedUpdated += Events_OnPlayerQueueIsReversedUpdated;
            _pamello.Events.OnPlayerQueueIsNoLeftoversUpdated += Events_OnPlayerQueueIsNoLeftoversUpdated;
            _pamello.Events.OnPlayerQueueIsFeedRandomUpdated += Events_OnPlayerQueueIsFeedRandomUpdated;

            _pamello.Events.OnPlayerNextPositionRequestUpdated += Events_OnPlayerNextPositionRequestUpdated;
            _pamello.Events.OnPlayerQueuePositionUpdated += Events_OnPlayerQueuePositionUpdated;
        }

        private async Task Events_OnSongDownloadProgeressUpdated(Core.Events.SongDownloadProgeressUpdated arg) {
            if (arg.SongId == _song?.Id) {
                RefreshCurrentSongDownloadProgress();
            }
        }
        private async Task Events_OnSongDownloadStarted(Core.Events.SongDownloadStarted arg) {
            if (arg.SongId == _song?.Id) {
                RefreshCurrentSongDownloadState();
            }
        }
        private async Task Events_OnSongDownloadFinished(Core.Events.SongDownloadFinished arg) {
            if (arg.SongId == _song?.Id) {
                RefreshCurrentSongDownloadState();
            }
        }

        private async Task Events_OnPlayerQueueSongsIdsUpdated(Core.Events.PlayerQueueEntriesDTOsUpdated arg) {
            RefreshPlayerQueueList();
        }

        private async Task Events_OnUserSelectedPlayerIdUpdated(Core.Events.UserSelectedPlayerIdUpdated arg) {
            if (arg.UserId == _pamello.Users.Current.Id) {
                await Update();
            }
        }

        private async Task Events_OnPlayerQueueIsFeedRandomUpdated(Core.Events.PlayerQueueIsFeedRandomUpdated arg) {
            Console.WriteLine($"1 {_player.QueueIsFeedRandom}");
            RefreshPlayerQueueIsFeedRandom();
        }
        private async Task Events_OnPlayerQueueIsRandomUpdated(Core.Events.PlayerQueueIsRandomUpdated arg) {
            Console.WriteLine($"2 {_player.QueueIsRandom}");
            RefreshPlayerQueueIsRandom();
        }
        private async Task Events_OnPlayerQueueIsNoLeftoversUpdated(Core.Events.PlayerQueueIsNoLeftoversUpdated arg) {
            Console.WriteLine($"3 {_player.QueueIsNoLeftovers}");
            RefreshPlayerQueueIsNoLeftovers();
        }
        private async Task Events_OnPlayerQueueIsReversedUpdated(Core.Events.PlayerQueueIsReversedUpdated arg) {
            Console.WriteLine($"4 {_player.QueueIsReversed}");
            RefreshPlayerQueueIsReversed();
        }

        private async Task Events_OnPlayerCurrentSongIdUpdated(Core.Events.PlayerCurrentSongIdUpdated arg) {
            await UpdateSong();

            RefreshPlayerCurrentSong();
        }

        private async Task Events_OnPlayerStateUpdated(Core.Events.PlayerStateUpdated arg) {
            RefreshPlayerState();
        }

        private async Task Events_OnPlayerCurrentSongTimeTotalUpdated(Core.Events.PlayerCurrentSongTimeTotalUpdated arg) {
            RefreshPlayerCurrentSongTimeTotal();
        }

        private async Task Events_OnPlayerCurrentSongTimePassedUpdated(Core.Events.PlayerCurrentSongTimePassedUpdated arg) {
            RefreshPlayerCurrentSongTimePassed();
        }

        private async Task Events_OnPlayerIsPausedUpdated(Core.Events.PlayerIsPausedUpdated arg) {
            RefreshPlayerIsPaused();
        }

        private async Task Events_OnPlayerNextPositionRequestUpdated(Core.Events.PlayerNextPositionRequestUpdated arg) {
            RefreshPlayerQueueList();
        }

        private async Task Events_OnPlayerQueuePositionUpdated(Core.Events.PlayerQueuePositionUpdated arg) {
            RefreshPlayerQueueList();
        }
    }
}
