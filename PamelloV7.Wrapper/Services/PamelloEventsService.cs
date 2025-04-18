using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

                Console.Write($"setting event string: \"{str}\"... ");

                if (parts[0] == "id") {
                    EventName = (EEventName)int.Parse(parts[1]);
                }
                else if (parts[0] == "data") {
                    Data = parts[1];
                }
                else {
                    Console.WriteLine("error");
                    return false;
                }
                Console.WriteLine("done");

                return true;
            }
        }

        private readonly HttpClient _http;
        private readonly PamelloClient _client;

        public Guid? EventsToken { get; internal set; }


        public event Func<Task>? OnConnection;

        public event Func<PamelloEvent, Task> OnPamelloEvent;

        public event Func<EventsConnected, Task> OnEventsConnected;
        public event Func<EventsAuthorized, Task> OnEventsAuthorized;
        public event Func<EventsUnAuthorized, Task> OnEventsUnAuthorized;

        public event Func<UserCreated, Task> OnUserCreated;
        public event Func<UserUpdated, Task> OnUserUpdated;
        public event Func<UserNameUpdated, Task> OnUserNameUpdated;
        public event Func<UserAvatarUpdated, Task> OnUserAvatarUpdated;
        public event Func<UserSelectedPlayerIdUpdated, Task> OnUserSelectedPlayerIdUpdated;
        public event Func<UserSongsPlayedUpdated, Task> OnUserSongsPlayedUpdated;
        public event Func<UserAddedSongsUpdated, Task> OnUserAddedSongsUpdated;
        public event Func<UserAddedPlaylistsUpdated, Task> OnUserAddedPlaylistsUpdated;
        public event Func<UserFavoriteSongsUpdated, Task> OnUserFavoriteSongsUpdated;
        public event Func<UserFavoritePlaylistsUpdated, Task> OnUserFavoritePlaylistsUpdated;
        public event Func<UserIsAdministratorUpdated, Task> OnUserIsAdministratorUpdated;

        public event Func<SongCreated, Task> OnSongCreated;
        public event Func<SongUpdated, Task> OnSongUpdated;
        public event Func<SongNameUpdated, Task> OnSongNameUpdated;
        public event Func<SongCoverUrlUpdated, Task> OnSongCoverUrlUpdated;
        public event Func<SongPlayCountUpdated, Task> OnSongPlayCountUpdated;
        public event Func<SongAssociacionsUpdated, Task> OnSongAssociacionsUpdated;
        public event Func<SongFavoriteByIdsUpdated, Task> OnSongFavoriteByIdsUpdated;
        public event Func<SongEpisodesIdsUpdated, Task> OnSongEpisodesIdsUpdated;
        public event Func<SongPlaylistsIdsUpdated, Task> OnSongPlaylistsIdsUpdated;
        public event Func<SongDownloadStarted, Task> OnSongDownloadStarted;
        public event Func<SongDownloadProgeressUpdated, Task> OnSongDownloadProgeressUpdated;
        public event Func<SongDownloadFinished, Task> OnSongDownloadFinished;

        public event Func<EpisodeCreated, Task> OnEpisodeCreated;
        public event Func<EpisodeUpdated, Task> OnEpisodeUpdated;
        public event Func<EpisodeDeleted, Task> OnEpisodeDeleted;
        public event Func<EpisodeNameUpdated, Task> OnEpisodeNameUpdated;
        public event Func<EpisodeStartUpdated, Task> OnEpisodeStartUpdated;
        public event Func<EpisodeSkipUpdated, Task> OnEpisodeSkipUpdated;

        public event Func<PlaylistCreated, Task> OnPlaylistCreated;
        public event Func<PlaylistUpdated, Task> OnPlaylistUpdated;
        public event Func<PlaylistDeleted, Task> OnPlaylistDeleted;
        public event Func<PlaylistNameUpdated, Task> OnPlaylistNameUpdated;
        public event Func<PlaylistProtectionUpdated, Task> OnPlaylistProtectionUpdated;
        public event Func<PlaylistSongsUpdated, Task> OnPlaylistSongsUpdated;
        public event Func<PlaylistFavoriteByIdsUpdated, Task> OnPlaylistFavoriteByIdsUpdated;

        public event Func<PlayerAvailable, Task> OnPlayerAvailable;
        public event Func<PlayerRemoved, Task> OnPlayerRemoved;
        public event Func<PlayerUpdated, Task> OnPlayerUpdated;
        public event Func<PlayerNameUpdated, Task> OnPlayerNameUpdated;
        public event Func<PlayerStateUpdated, Task> OnPlayerStateUpdated;
        public event Func<PlayerIsPausedUpdated, Task> OnPlayerIsPausedUpdated;
        public event Func<PlayerProtectionUpdated, Task> OnPlayerProtectionUpdated;
        public event Func<PlayerCurrentSongIdUpdated, Task> OnPlayerCurrentSongIdUpdated;
        public event Func<PlayerQueueEntriesDTOsUpdated, Task> OnPlayerQueueEntriesDTOsUpdated;
        public event Func<PlayerQueuePositionUpdated, Task> OnPlayerQueuePositionUpdated;
        public event Func<PlayerCurrentEpisodePositionUpdated, Task> OnPlayerCurrentEpisodePositionUpdated;
        public event Func<PlayerNextPositionRequestUpdated, Task> OnPlayerNextPositionRequestUpdated;
        public event Func<PlayerCurrentSongTimePassedUpdated, Task> OnPlayerCurrentSongTimePassedUpdated;
        public event Func<PlayerCurrentSongTimeTotalUpdated, Task> OnPlayerCurrentSongTimeTotalUpdated;
        public event Func<PlayerQueueIsRandomUpdated, Task> OnPlayerQueueIsRandomUpdated;
        public event Func<PlayerQueueIsReversedUpdated, Task> OnPlayerQueueIsReversedUpdated;
        public event Func<PlayerQueueIsNoLeftoversUpdated, Task> OnPlayerQueueIsNoLeftoversUpdated;
        public event Func<PlayerQueueIsFeedRandomUpdated, Task> OnPlayerQueueIsFeedRandomUpdated;

        public PamelloEventsService(PamelloClient client) {
            _client = client;

            _http = new HttpClient();

            OnPamelloEvent += PamelloEventsService_OnPamelloEvent;

            OnEventsConnected += PamelloEventsService_OnEventsConnected;
        }

        private async Task PamelloEventsService_OnEventsConnected(EventsConnected arg) {
            if (arg.EventsToken == Guid.Empty) return;

            EventsToken = arg.EventsToken;
            if (OnConnection is not null) await OnConnection.Invoke();
        }

        private async Task PamelloEventsService_OnPamelloEvent(PamelloEvent pamelloEvent) {
            switch (pamelloEvent.EventName) {
                case EEventName.EventsConnected:
                    await OnEventsConnected.Invoke((EventsConnected)pamelloEvent);
                    break;
                case EEventName.EventsAuthorized:
                    await OnEventsAuthorized.Invoke((EventsAuthorized)pamelloEvent);
                    break;
                case EEventName.EventsUnAuthorized:
                    await OnEventsUnAuthorized.Invoke((EventsUnAuthorized)pamelloEvent);
                    break;
                case EEventName.UserCreated:
                    await OnUserCreated.Invoke((UserCreated)pamelloEvent);
                    break;
                case EEventName.UserUpdated:
                    await OnUserUpdated.Invoke((UserUpdated)pamelloEvent);
                    break;
                case EEventName.UserNameUpdated:
                    await OnUserNameUpdated.Invoke((UserNameUpdated)pamelloEvent);
                    break;
                case EEventName.UserAvatarUpdated:
                    await OnUserAvatarUpdated.Invoke((UserAvatarUpdated)pamelloEvent);
                    break;
                case EEventName.UserSelectedPlayerIdUpdated:
                    await OnUserSelectedPlayerIdUpdated.Invoke((UserSelectedPlayerIdUpdated)pamelloEvent);
                    break;
                case EEventName.UserSongsPlayedUpdated:
                    await OnUserSongsPlayedUpdated.Invoke((UserSongsPlayedUpdated)pamelloEvent);
                    break;
                case EEventName.UserAddedSongsUpdated:
                    await OnUserAddedSongsUpdated.Invoke((UserAddedSongsUpdated)pamelloEvent);
                    break;
                case EEventName.UserAddedPlaylistsUpdated:
                    await OnUserAddedPlaylistsUpdated.Invoke((UserAddedPlaylistsUpdated)pamelloEvent);
                    break;
                case EEventName.UserFavoriteSongsUpdated:
                    await OnUserFavoriteSongsUpdated.Invoke((UserFavoriteSongsUpdated)pamelloEvent);
                    break;
                case EEventName.UserFavoritePlaylistsUpdated:
                    await OnUserFavoritePlaylistsUpdated.Invoke((UserFavoritePlaylistsUpdated)pamelloEvent);
                    break;
                case EEventName.UserIsAdministratorUpdated:
                    await OnUserIsAdministratorUpdated.Invoke((UserIsAdministratorUpdated)pamelloEvent);
                    break;
                case EEventName.SongCreated:
                    await OnSongCreated.Invoke((SongCreated)pamelloEvent);
                    break;
                case EEventName.SongUpdated:
                    await OnSongUpdated.Invoke((SongUpdated)pamelloEvent);
                    break;
                case EEventName.SongNameUpdated:
                    await OnSongNameUpdated.Invoke((SongNameUpdated)pamelloEvent);
                    break;
                case EEventName.SongCoverUrlUpdated:
                    await OnSongCoverUrlUpdated.Invoke((SongCoverUrlUpdated)pamelloEvent);
                    break;
                case EEventName.SongPlayCountUpdated:
                    await OnSongPlayCountUpdated.Invoke((SongPlayCountUpdated)pamelloEvent);
                    break;
                case EEventName.SongAssociacionsUpdated:
                    await OnSongAssociacionsUpdated.Invoke((SongAssociacionsUpdated)pamelloEvent);
                    break;
                case EEventName.SongFavoriteByIdsUpdated:
                    await OnSongFavoriteByIdsUpdated.Invoke((SongFavoriteByIdsUpdated)pamelloEvent);
                    break;
                case EEventName.SongEpisodesIdsUpdated:
                    await OnSongEpisodesIdsUpdated.Invoke((SongEpisodesIdsUpdated)pamelloEvent);
                    break;
                case EEventName.SongPlaylistsIdsUpdated:
                    await OnSongPlaylistsIdsUpdated.Invoke((SongPlaylistsIdsUpdated)pamelloEvent);
                    break;
                case EEventName.SongDownloadStarted:
                    await OnSongDownloadStarted.Invoke((SongDownloadStarted)pamelloEvent);
                    break;
                case EEventName.SongDownloadProgeressUpdated:
                    await OnSongDownloadProgeressUpdated.Invoke((SongDownloadProgeressUpdated)pamelloEvent);
                    break;
                case EEventName.SongDownloadFinished:
                    await OnSongDownloadFinished.Invoke((SongDownloadFinished)pamelloEvent);
                    break;
                case EEventName.EpisodeCreated:
                    await OnEpisodeCreated.Invoke((EpisodeCreated)pamelloEvent);
                    break;
                case EEventName.EpisodeUpdated:
                    await OnEpisodeUpdated.Invoke((EpisodeUpdated)pamelloEvent);
                    break;
                case EEventName.EpisodeDeleted:
                    await OnEpisodeDeleted.Invoke((EpisodeDeleted)pamelloEvent);
                    break;
                case EEventName.EpisodeNameUpdated:
                    await OnEpisodeNameUpdated.Invoke((EpisodeNameUpdated)pamelloEvent);
                    break;
                case EEventName.EpisodeStartUpdated:
                    await OnEpisodeStartUpdated.Invoke((EpisodeStartUpdated)pamelloEvent);
                    break;
                case EEventName.EpisodeSkipUpdated:
                    await OnEpisodeSkipUpdated.Invoke((EpisodeSkipUpdated)pamelloEvent);
                    break;
                case EEventName.PlaylistCreated:
                    await OnPlaylistCreated.Invoke((PlaylistCreated)pamelloEvent);
                    break;
                case EEventName.PlaylistUpdated:
                    await OnPlaylistUpdated.Invoke((PlaylistUpdated)pamelloEvent);
                    break;
                case EEventName.PlaylistDeleted:
                    await OnPlaylistDeleted.Invoke((PlaylistDeleted)pamelloEvent);
                    break;
                case EEventName.PlaylistNameUpdated:
                    await OnPlaylistNameUpdated.Invoke((PlaylistNameUpdated)pamelloEvent);
                    break;
                case EEventName.PlaylistProtectionUpdated:
                    await OnPlaylistProtectionUpdated.Invoke((PlaylistProtectionUpdated)pamelloEvent);
                    break;
                case EEventName.PlaylistSongsUpdated:
                    await OnPlaylistSongsUpdated.Invoke((PlaylistSongsUpdated)pamelloEvent);
                    break;
                case EEventName.PlaylistFavoriteByIdsUpdated:
                    await OnPlaylistFavoriteByIdsUpdated.Invoke((PlaylistFavoriteByIdsUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerAvailable:
                    await OnPlayerAvailable.Invoke((PlayerAvailable)pamelloEvent);
                    break;
                case EEventName.PlayerRemoved:
                    await OnPlayerRemoved.Invoke((PlayerRemoved)pamelloEvent);
                    break;
                case EEventName.PlayerUpdated:
                    await OnPlayerUpdated.Invoke((PlayerUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerNameUpdated:
                    await OnPlayerNameUpdated.Invoke((PlayerNameUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerStateUpdated:
                    await OnPlayerStateUpdated.Invoke((PlayerStateUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerIsPausedUpdated:
                    await OnPlayerIsPausedUpdated.Invoke((PlayerIsPausedUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerProtectionUpdated:
                    await OnPlayerProtectionUpdated.Invoke((PlayerProtectionUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerCurrentSongIdUpdated:
                    await OnPlayerCurrentSongIdUpdated.Invoke((PlayerCurrentSongIdUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerQueueEntriesDTOsUpdated:
                    await OnPlayerQueueEntriesDTOsUpdated.Invoke((PlayerQueueEntriesDTOsUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerQueuePositionUpdated:
                    await OnPlayerQueuePositionUpdated.Invoke((PlayerQueuePositionUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerCurrentEpisodePositionUpdated:
                    await OnPlayerCurrentEpisodePositionUpdated.Invoke((PlayerCurrentEpisodePositionUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerNextPositionRequestUpdated:
                    await OnPlayerNextPositionRequestUpdated.Invoke((PlayerNextPositionRequestUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerCurrentSongTimePassedUpdated:
                    await OnPlayerCurrentSongTimePassedUpdated.Invoke((PlayerCurrentSongTimePassedUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerCurrentSongTimeTotalUpdated:
                    await OnPlayerCurrentSongTimeTotalUpdated.Invoke((PlayerCurrentSongTimeTotalUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerQueueIsRandomUpdated:
                    await OnPlayerQueueIsRandomUpdated.Invoke((PlayerQueueIsRandomUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerQueueIsReversedUpdated:
                    await OnPlayerQueueIsReversedUpdated.Invoke((PlayerQueueIsReversedUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerQueueIsNoLeftoversUpdated:
                    await OnPlayerQueueIsNoLeftoversUpdated.Invoke((PlayerQueueIsNoLeftoversUpdated)pamelloEvent);
                    break;
                case EEventName.PlayerQueueIsFeedRandomUpdated:
                    await OnPlayerQueueIsFeedRandomUpdated.Invoke((PlayerQueueIsFeedRandomUpdated)pamelloEvent);
                    break;
            }
        }

        public async Task<bool> Connect(string? serverHost = null) {
            if (serverHost is null) serverHost = _client.ServerHost;

            var eventStream = await _http.GetStreamAsync($"http://{serverHost}/Events");
            if (eventStream is null) return false;

            _client.ServerHost = serverHost;

            Task.Run(() => ListenEventStream(eventStream));

            return true;
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
            Console.WriteLine("reading event");

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
                case EEventName.EventsAuthorized:
                    pamelloEvent = JsonSerializer.Deserialize<EventsAuthorized>(sseEvent.Data);
                    break;
                case EEventName.EventsUnAuthorized:
                    pamelloEvent = JsonSerializer.Deserialize<EventsUnAuthorized>(sseEvent.Data);
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
                case EEventName.SongPlayCountUpdated:
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
                case EEventName.PlayerQueueEntriesDTOsUpdated:
                    pamelloEvent = JsonSerializer.Deserialize<PlayerQueueEntriesDTOsUpdated>(sseEvent.Data);
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

        public async Task<bool> Authorize() {
            if (EventsToken is null) return false;
            if (_client.Authorization.UserToken is null) return false;

            await _client.HttpGetAsync($"Authorization/Events/{EventsToken}/WithToken/{_client.Authorization.UserToken}");

            return true;
        }
        public async Task UnAuthorize() {
            if (EventsToken is null) return;

            await _client.HttpGetAsync($"Authorization/Events/{EventsToken}/Unauthorize");
        }
    }
}