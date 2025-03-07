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

        public PamelloEventsService(PamelloClient pamelloClient) {
            _pamelloClient = pamelloClient;

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
                    break;
                case EEventName.UserUpdated:
                    break;
                case EEventName.UserNameUpdated:
                    break;
                case EEventName.UserAvatarUpdated:
                    break;
                case EEventName.UserSelectedPlayerIdUpdated:
                    break;
                case EEventName.UserSongsPlayedUpdated:
                    break;
                case EEventName.UserAddedSongsUpdated:
                    break;
                case EEventName.UserAddedPlaylistsUpdated:
                    break;
                case EEventName.UserFavoriteSongsUpdated:
                    break;
                case EEventName.UserFavoritePlaylistsUpdated:
                    break;
                case EEventName.UserIsAdministratorUpdated:
                    break;
                case EEventName.SongCreated:
                    break;
                case EEventName.SongUpdated:
                    break;
                case EEventName.SongNameUpdated:
                    break;
                case EEventName.SongCoverUrlUpdated:
                    break;
                case EEventName.SongPlaycountUpdated:
                    break;
                case EEventName.SongAssociacionsUpdated:
                    break;
                case EEventName.SongFavoriteByIdsUpdated:
                    break;
                case EEventName.SongEpisodesIdsUpdated:
                    break;
                case EEventName.SongPlaylistsIdsUpdated:
                    break;
                case EEventName.SongDownloadStarted:
                    break;
                case EEventName.SongDownloadProgeressUpdated:
                    break;
                case EEventName.SongDownloadFinished:
                    break;
                case EEventName.EpisodeCreated:
                    break;
                case EEventName.EpisodeUpdated:
                    break;
                case EEventName.EpisodeDeleted:
                    break;
                case EEventName.EpisodeNameUpdated:
                    break;
                case EEventName.EpisodeStartUpdated:
                    break;
                case EEventName.EpisodeSkipUpdated:
                    break;
                case EEventName.PlaylistCreated:
                    break;
                case EEventName.PlaylistUpdated:
                    break;
                case EEventName.PlaylistDeleted:
                    break;
                case EEventName.PlaylistNameUpdated:
                    break;
                case EEventName.PlaylistProtectionUpdated:
                    break;
                case EEventName.PlaylistSongsUpdated:
                    break;
                case EEventName.PlaylistFavoriteByIdsUpdated:
                    break;
                case EEventName.PlayerAvailable:
                    break;
                case EEventName.PlayerRemoved:
                    break;
                case EEventName.PlayerUpdated:
                    break;
                case EEventName.PlayerNameUpdated:
                    break;
                case EEventName.PlayerStateUpdated:
                    break;
                case EEventName.PlayerIsPausedUpdated:
                    break;
                case EEventName.PlayerProtectionUpdated:
                    break;
                case EEventName.PlayerCurrentSongIdUpdated:
                    break;
                case EEventName.PlayerQueueSongsIdsUpdated:
                    break;
                case EEventName.PlayerQueuePositionUpdated:
                    break;
                case EEventName.PlayerCurrentEpisodePositionUpdated:
                    break;
                case EEventName.PlayerNextPositionRequestUpdated:
                    break;
                case EEventName.PlayerCurrentSongTimePassedUpdated:
                    break;
                case EEventName.PlayerCurrentSongTimeTotalUpdated:
                    break;
                case EEventName.PlayerQueueIsRandomUpdated:
                    break;
                case EEventName.PlayerQueueIsReversedUpdated:
                    break;
                case EEventName.PlayerQueueIsNoLeftoversUpdated:
                    break;
                case EEventName.PlayerQueueIsFeedRandomUpdated:
                    break;
            }

            return pamelloEvent;
        }
    }
}