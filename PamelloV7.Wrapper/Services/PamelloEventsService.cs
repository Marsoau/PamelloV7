using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.ServerSentEvents;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Services
{
    public class PamelloEventsService
    {
        private class SseEvent {
            public EEventName EventName { get; set; }
            public string Data { get; set; }

            public bool SetString(string str) {
                var parts = str.Split(": ");

                if (parts[0] == "id") {
                    EventName = (EEventName)int.Parse(parts[1]);
                }
                else if (parts[0] == "data") {
                    Data = parts[1];
                }
                else return false;

                return true;
            }
        }

        private readonly HttpClient _http;
        private readonly PamelloClient _pamelloClient;

        public event Func<Task>? OnConnection;

        public event Func<PamelloEvent, Task> OnPamelloEvent;

        public PamelloEventsService(PamelloClient client) {
            _pamelloClient = client;

            _http = new HttpClient();

            OnPamelloEvent += PamelloEventsService_OnPamelloEvent;
        }

        private async Task PamelloEventsService_OnPamelloEvent(PamelloEvent pamelloEvent) {
            if (pamelloEvent is EventsConnected eventsConnected) {
                _pamelloClient.EventsToken = eventsConnected.EventsToken;
                OnConnection?.Invoke();
            }
        }

        public async Task Connect(string serverHost) {
            Stream? eventStream = null;

            eventStream = await _http.GetStreamAsync($"http://{serverHost}/Events");
            if (eventStream is null) throw new Exception("Cant connect");

            _pamelloClient.ServerHost = serverHost;
            Task.Run(() => ListenEventStream(eventStream));
        }
        public async Task<bool> TryConnect(string serverHost) {
            try {
                await Connect(serverHost);
            }
            catch {
                return false;
            }

            return true;
        }

        private void ListenEventStream(Stream eventStream) {
            var sr = new StreamReader(eventStream);

            SseEvent? sseEvent;
            PamelloEvent? pamelloEvent;

            while (!sr.EndOfStream) {
                sseEvent = ReadEvent(sr);
                if (sseEvent is null) continue;

                pamelloEvent = ConvertToPamelloEvent(sseEvent);
                if (pamelloEvent is null) continue;

                OnPamelloEvent.Invoke(pamelloEvent);
            }
        }

        private SseEvent? ReadEvent(StreamReader sr) {
            char[] buffer = new char[1];
            StringBuilder sb = new StringBuilder();

            var brakeFound = false;

            SseEvent? sseEvent = null;

            while (!sr.EndOfStream) {
                sr.Read(buffer, 0, 1);

                if (buffer[0] == '\r') {
                    if (brakeFound) {
                        return sseEvent;
                    }
                    else {
                        brakeFound = true;

                        if (sseEvent is null) {
                            sseEvent = new SseEvent();
                            if (!sseEvent.SetString(sb.ToString())) {
                                sseEvent = null;
                            }
                        }
                        else sseEvent.SetString(sb.ToString());

                        sb.Clear();
                    }
                }
                else {
                    brakeFound = false;

                    sb.Append(buffer[0]);
                }
            }

            return sseEvent;
        }

        private PamelloEvent? ConvertToPamelloEvent(SseEvent sseEvent) {
            PamelloEvent? pamelloEvent = null;

            switch (sseEvent.EventName) {
                case EEventName.EventsConnected:
                    pamelloEvent = JsonSerializer.Deserialize<EventsConnected>(sseEvent.Data);
                    break;
                case EEventName.UserCreated:
                    pamelloEvent = JsonSerializer.Deserialize<UserCreated>(sseEvent.Data);
                    break;
                case EEventName.UserUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserUpdated>(sseEvent.Data);
                    break;
                case EEventName.UserNameUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserNameUpdated>(sseEvent.Data);
                    break;
                case EEventName.UserAvatarUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserAvatarUpdated>(sseEvent.Data);
                    break;
                case EEventName.UserSelectedPlayerIdUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserSelectedPlayerIdUpdated>(sseEvent.Data);
                    break;
                case EEventName.UserSongsPlayedUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserSongsPlayedUpdated>(sseEvent.Data);
                    break;
                case EEventName.UserAddedSongsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserAddedSongsUpdated>(sseEvent.Data);
                    break;
                case EEventName.UserAddedPlaylistsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserAddedPlaylistsUpdated>(sseEvent.Data);
                    break;
                case EEventName.UserFavoriteSongsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserFavoriteSongsUpdated>(sseEvent.Data);
                    break;
                case EEventName.UserFavoritePlaylistsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserFavoritePlaylistsUpdated>(sseEvent.Data);
                    break;
                case EEventName.UserIsAdministratorUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<UserIsAdministratorUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongCreated:
                    pamelloEvent = JsonSerializer.Deserialize<SongCreated>(sseEvent.Data);
                    break;
                case EEventName.SongUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<SongUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongNameUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<SongNameUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongCoverUrlUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<SongCoverUrlUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongPlaycountUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<SongPlayCountUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongAssociacionsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<SongAssociacionsUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongFavoriteByIdsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<SongFavoriteByIdsUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongEpisodesIdsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<SongEpisodesIdsUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongPlaylistsIdsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<SongPlaylistsIdsUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongDownloadStarted:
                    pamelloEvent = JsonSerializer.Deserialize<SongDownloadStarted>(sseEvent.Data);
                    break;
                case EEventName.SongDownloadProgeressUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<SongDownloadProgeressUpdated>(sseEvent.Data);
                    break;
                case EEventName.SongDownloadFinished:
                    pamelloEvent = JsonSerializer.Deserialize<SongDownloadFinished>(sseEvent.Data);
                    break;
                case EEventName.EpisodeCreated:
                    pamelloEvent = JsonSerializer.Deserialize<EpisodeCreated>(sseEvent.Data);
                    break;
                case EEventName.EpisodeUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<EpisodeUpdated>(sseEvent.Data);
                    break;
                case EEventName.EpisodeDeleted:
                    pamelloEvent = JsonSerializer.Deserialize<EpisodeDeleted>(sseEvent.Data);
                    break;
                case EEventName.EpisodeNameUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<EpisodeNameUpdated>(sseEvent.Data);
                    break;
                case EEventName.EpisodeStartUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<EpisodeStartUpdated>(sseEvent.Data);
                    break;
                case EEventName.EpisodeSkipUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<EpisodeSkipUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlaylistCreated:
                    pamelloEvent = JsonSerializer.Deserialize<PlaylistCreated>(sseEvent.Data);
                    break;
                case EEventName.PlaylistUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlaylistUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlaylistDeleted:
                    pamelloEvent = JsonSerializer.Deserialize<PlaylistDeleted>(sseEvent.Data);
                    break;
                case EEventName.PlaylistNameUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlaylistNameUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlaylistProtectionUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlaylistProtectionUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlaylistSongsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlaylistSongsUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlaylistFavoriteByIdsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlaylistFavoriteByIdsUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerAvailable:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerAvailable>(sseEvent.Data);
                    break;
                case EEventName.PlayerRemoved:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerRemoved>(sseEvent.Data);
                    break;
                case EEventName.PlayerUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerNameUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerNameUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerStateUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerStateUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerIsPausedUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerIsPausedUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerProtectionUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerProtectionUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerCurrentSongIdUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerCurrentSongIdUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerQueueSongsIdsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerQueueSongsIdsUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerQueuePositionUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerQueuePositionUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerCurrentEpisodePositionUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerCurrentEpisodePositionUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerNextPositionRequestUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerNextPositionRequestUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerCurrentSongTimePassedUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerCurrentSongTimePassedUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerCurrentSongTimeTotalUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerCurrentSongTimeTotalUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerQueueIsRandomUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerQueueIsRandomUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerQueueIsReversedUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerQueueIsReversedUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerQueueIsNoLeftoversUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerQueueIsNoLeftoversUpdated>(sseEvent.Data);
                    break;
                case EEventName.PlayerQueueIsFeedRandomUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerQueueIsFeedRandomUpdated>(sseEvent.Data);
                    break;
            }

            return pamelloEvent;
        }
    }
}