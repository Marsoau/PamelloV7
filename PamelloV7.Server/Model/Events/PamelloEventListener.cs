using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using System.Text.Json;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Server.Model.Events
{
    public class PamelloEventListener
    {
        private readonly HttpResponse _response;

        public Guid Token { get; }
        public PamelloUser? User { get; private set; }

        public bool IsAuthorized { get => User is not null; }
        public bool IsClosed { get; private set; }

        public readonly Queue<PamelloEvent> _eventsQueue;

        private event Action OnNewEventAdded;

        private readonly AutoResetEvent _eventsWait;

        public PamelloEventListener(HttpResponse response) {
            _response = response;

            Token = Guid.NewGuid();

            _eventsQueue = new Queue<PamelloEvent>();

            _eventsWait = new AutoResetEvent(false);

            Task.Run(EventsSendingThread);
        }

        private async Task EventsSendingThread() {
            while (true) {
                _eventsWait.WaitOne();
                await SendAllEventsAsync();
                _eventsWait.Reset();
            }
        }

        private async Task SendAllEventsAsync() {
            PamelloEvent pamelloEvent;

            while (_eventsQueue.Count > 0) {
                pamelloEvent = _eventsQueue.Dequeue();

                await _response.WriteAsync($"id: {(int)pamelloEvent.EventName}\revent: {pamelloEvent.EventName}\r");
                switch (pamelloEvent.EventName) {
                    case EEventName.EventsConnected:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((EventsConnected)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserCreated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserCreated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserNameUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserNameUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserAvatarUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserAvatarUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserSelectedPlayerIdUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserSelectedPlayerIdUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserSongsPlayedUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserSongsPlayedUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserAddedSongsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserAddedSongsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserAddedPlaylistsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserAddedPlaylistsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserFavoriteSongsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserFavoriteSongsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserFavoritePlaylistsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserFavoritePlaylistsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.UserIsAdministratorUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((UserIsAdministratorUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongCreated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongCreated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongNameUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongNameUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongCoverUrlUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongCoverUrlUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongPlayCountUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongPlayCountUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongAssociacionsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongAssociacionsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongFavoriteByIdsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongFavoriteByIdsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongEpisodesIdsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongEpisodesIdsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongPlaylistsIdsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongPlaylistsIdsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongDownloadStarted:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongDownloadStarted)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongDownloadProgeressUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongDownloadProgeressUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.SongDownloadFinished:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((SongDownloadFinished)pamelloEvent)}\r\r");
                        break;
                    case EEventName.EpisodeCreated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((EpisodeCreated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.EpisodeUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((EpisodeUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.EpisodeDeleted:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((EpisodeDeleted)pamelloEvent)}\r\r");
                        break;
                    case EEventName.EpisodeNameUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((EpisodeNameUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.EpisodeStartUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((EpisodeStartUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.EpisodeSkipUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((EpisodeSkipUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlaylistCreated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlaylistCreated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlaylistUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlaylistUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlaylistDeleted:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlaylistDeleted)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlaylistNameUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlaylistNameUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlaylistProtectionUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlaylistProtectionUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlaylistSongsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlaylistSongsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlaylistFavoriteByIdsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlaylistFavoriteByIdsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerAvailable:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerAvailable)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerRemoved:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerRemoved)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerNameUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerNameUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerStateUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerStateUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerIsPausedUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerIsPausedUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerProtectionUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerProtectionUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerCurrentSongIdUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerCurrentSongIdUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerQueueEntriesDTOsUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerQueueEntriesDTOsUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerQueuePositionUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerQueuePositionUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerCurrentEpisodePositionUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerCurrentEpisodePositionUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerNextPositionRequestUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerNextPositionRequestUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerCurrentSongTimePassedUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerCurrentSongTimePassedUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerCurrentSongTimeTotalUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerCurrentSongTimeTotalUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerQueueIsRandomUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerQueueIsRandomUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerQueueIsReversedUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerQueueIsReversedUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerQueueIsNoLeftoversUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerQueueIsNoLeftoversUpdated)pamelloEvent)}\r\r");
                        break;
                    case EEventName.PlayerQueueIsFeedRandomUpdated:
                        await _response.WriteAsync($"data: {JsonSerializer.Serialize((PlayerQueueIsFeedRandomUpdated)pamelloEvent)}\r\r");
                        break;
                }

                await _response.Body.FlushAsync();
            }
        }

        public async Task InitializeConnecion() {
            _response.ContentType = "text/event-stream";
            _response.Headers.CacheControl = "no-cache";
            await _response.Body.FlushAsync();

            ScheduleEvent(new EventsConnected() {
                EventsToken = Token,
            });
        }

        private static int eventN = 0;
        public void ScheduleEvent<TEventType>(TEventType pamelloEvent)
            where TEventType : PamelloEvent
        {
            if (IsClosed) return;

            _eventsQueue.Enqueue(pamelloEvent);
            _eventsWait.Set();
        }

        public void AssighnUser(PamelloUser user) {
            User = user;
        }

        public void Close() {
            IsClosed = true;
        }
    }
}
