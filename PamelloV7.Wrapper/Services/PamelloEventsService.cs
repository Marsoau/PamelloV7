using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PamelloV7.Core.Exceptions;

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
                else {
                    return false;
                }

                return true;
            }
        }

        private readonly PamelloClient _client;

        private HttpClient? _http;
        private Stream? _eventStream;

        public Guid? EventsToken { get; internal set; }

        public bool IsConnected { get; private set; }
        public bool AutoReconnect { get; set; }
        public int ReconnectDelay { get; set; }
        
        private CancellationTokenSource _eventsCTS;
        private CancellationTokenSource _reconnectionCTS;
        private Task? _eventStreamTask;
        private Task? _reconnectionTask;

        public event Func<Task>? OnConnect;
        public event Func<Task> OnDisconnect;
        public event Action<int>? OnReconnectAttepmt;

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

            _http = null;
            _eventStream = null;
            
            _eventsCTS = new CancellationTokenSource();
            _reconnectionCTS = new CancellationTokenSource();

            OnPamelloEvent += PamelloEventsService_OnPamelloEvent;

            OnEventsConnected += PamelloEventsService_OnEventsConnected;

            OnDisconnect += PamelloEventsService_OnDisconnect;

            IsConnected = false;
            
            AutoReconnect = false;
            ReconnectDelay = 3000;
        }

        private async Task PamelloEventsService_OnDisconnect() {
            await _client.Cleanup();
            
            if (AutoReconnect) {
                _reconnectionTask = ReconnectionLoop();
            }
        }

        private async Task PamelloEventsService_OnEventsConnected(EventsConnected arg) {
            if (arg.EventsToken == Guid.Empty) return;

            EventsToken = arg.EventsToken;
            
            IsConnected = true;
            if (OnConnect is not null) await OnConnect.Invoke();
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

        public async Task<bool> TryConnect(string serverHost) {
            try {
                await Connect(serverHost);
            }
            catch {
                return false;
            }

            return true;
        }

        public async Task ConnectRequired(string? serverHost = null) {
            _reconnectionCTS.Cancel();
            if (_reconnectionTask is not null) await _reconnectionTask;
            _reconnectionCTS = new CancellationTokenSource();
            
            if (serverHost is not null) _client.ServerHost = serverHost;
            
            _reconnectionTask = ReconnectionLoop();
        }
        
        public async Task<bool> Connect(string? serverHost = null) {
            if (serverHost is null) serverHost = _client.ServerHost;
            if (!await _client.CheckConnection()) return false;

            if (_http is not null || _eventStream is not null) {
                _http?.Dispose();
                _eventStream?.Dispose();
            }
            
            _http = new HttpClient();

            try {
                _eventStream = await _http.GetStreamAsync($"http://{serverHost}/Events");
            }
            catch {
                return false;
            }
            
            if (_eventStream is null) return false;

            _client.ServerHost = serverHost;

            _eventStreamTask = Task.Run(() => ListenEventStream(_eventStream));

            return true;
        }

        public async Task ReconnectionLoop() {
            var count = 0;
            try {
                while (!_reconnectionCTS.IsCancellationRequested && !await Connect()) {
                    OnReconnectAttepmt?.Invoke(count++);
                    await Task.Delay(ReconnectDelay);
                }
            }
            catch {
            }
        }

        public async Task Disconnect() {
            await _client.HttpGetAsync($"Events/{EventsToken}/Close");
        }

        private void ListenEventStream(Stream eventStream) {
            var sr = new StreamReader(eventStream);

            SseEvent? sseEvent;
            PamelloEvent? pamelloEvent;

            while (!sr.EndOfStream && !_eventsCTS.IsCancellationRequested) {
                sseEvent = ReadEvent(sr);
                if (sseEvent is null) continue;

                pamelloEvent = ConvertToPamelloEvent(sseEvent);
                if (pamelloEvent is null) continue;

                OnPamelloEvent.Invoke(pamelloEvent);
            }

            IsConnected = false;
            OnDisconnect.Invoke();
        }

        private SseEvent? ReadEvent(StreamReader sr) {
            var buffer = new char[1];
            var sb = new StringBuilder();

            var brakeFound = false;

            SseEvent? sseEvent = null;

            while (!sr.EndOfStream && !_eventsCTS.IsCancellationRequested) {
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
            PamelloEvent? pamelloEvent = sseEvent.EventName switch {
                EEventName.EventsConnected => JsonSerializer.Deserialize<EventsConnected>(sseEvent.Data),
                EEventName.EventsAuthorized => JsonSerializer.Deserialize<EventsAuthorized>(sseEvent.Data),
                EEventName.EventsUnAuthorized => JsonSerializer.Deserialize<EventsUnAuthorized>(sseEvent.Data),
                EEventName.UserCreated => JsonSerializer.Deserialize<UserCreated>(sseEvent.Data),
                EEventName.UserUpdated => JsonSerializer.Deserialize<UserUpdated>(sseEvent.Data),
                EEventName.UserNameUpdated => JsonSerializer.Deserialize<UserNameUpdated>(sseEvent.Data),
                EEventName.UserAvatarUpdated => JsonSerializer.Deserialize<UserAvatarUpdated>(sseEvent.Data),
                EEventName.UserSelectedPlayerIdUpdated => JsonSerializer.Deserialize<UserSelectedPlayerIdUpdated>(sseEvent.Data),
                EEventName.UserSongsPlayedUpdated => JsonSerializer.Deserialize<UserSongsPlayedUpdated>(sseEvent.Data),
                EEventName.UserAddedSongsUpdated => JsonSerializer.Deserialize<UserAddedSongsUpdated>(sseEvent.Data),
                EEventName.UserAddedPlaylistsUpdated => JsonSerializer.Deserialize<UserAddedPlaylistsUpdated>(sseEvent.Data),
                EEventName.UserFavoriteSongsUpdated => JsonSerializer.Deserialize<UserFavoriteSongsUpdated>(sseEvent.Data),
                EEventName.UserFavoritePlaylistsUpdated => JsonSerializer.Deserialize<UserFavoritePlaylistsUpdated>(sseEvent.Data),
                EEventName.UserIsAdministratorUpdated => JsonSerializer.Deserialize<UserIsAdministratorUpdated>(sseEvent.Data),
                EEventName.SongCreated => JsonSerializer.Deserialize<SongCreated>(sseEvent.Data),
                EEventName.SongUpdated => JsonSerializer.Deserialize<SongUpdated>(sseEvent.Data),
                EEventName.SongNameUpdated => JsonSerializer.Deserialize<SongNameUpdated>(sseEvent.Data),
                EEventName.SongCoverUrlUpdated => JsonSerializer.Deserialize<SongCoverUrlUpdated>(sseEvent.Data),
                EEventName.SongPlayCountUpdated => JsonSerializer.Deserialize<SongPlayCountUpdated>(sseEvent.Data),
                EEventName.SongAssociacionsUpdated => JsonSerializer.Deserialize<SongAssociacionsUpdated>(sseEvent.Data),
                EEventName.SongFavoriteByIdsUpdated => JsonSerializer.Deserialize<SongFavoriteByIdsUpdated>(sseEvent.Data),
                EEventName.SongEpisodesIdsUpdated => JsonSerializer.Deserialize<SongEpisodesIdsUpdated>(sseEvent.Data),
                EEventName.SongPlaylistsIdsUpdated =>JsonSerializer.Deserialize<SongPlaylistsIdsUpdated>(sseEvent.Data),
                EEventName.SongDownloadStarted => JsonSerializer.Deserialize<SongDownloadStarted>(sseEvent.Data),
                EEventName.SongDownloadProgeressUpdated => JsonSerializer.Deserialize<SongDownloadProgeressUpdated>(sseEvent.Data),
                EEventName.SongDownloadFinished => JsonSerializer.Deserialize<SongDownloadFinished>(sseEvent.Data),
                EEventName.EpisodeCreated => JsonSerializer.Deserialize<EpisodeCreated>(sseEvent.Data),
                EEventName.EpisodeUpdated => JsonSerializer.Deserialize<EpisodeUpdated>(sseEvent.Data),
                EEventName.EpisodeDeleted => JsonSerializer.Deserialize<EpisodeDeleted>(sseEvent.Data),
                EEventName.EpisodeNameUpdated => JsonSerializer.Deserialize<EpisodeNameUpdated>(sseEvent.Data),
                EEventName.EpisodeStartUpdated => JsonSerializer.Deserialize<EpisodeStartUpdated>(sseEvent.Data),
                EEventName.EpisodeSkipUpdated => JsonSerializer.Deserialize<EpisodeSkipUpdated>(sseEvent.Data),
                EEventName.PlaylistCreated => JsonSerializer.Deserialize<PlaylistCreated>(sseEvent.Data),
                EEventName.PlaylistUpdated => JsonSerializer.Deserialize<PlaylistUpdated>(sseEvent.Data),
                EEventName.PlaylistDeleted => JsonSerializer.Deserialize<PlaylistDeleted>(sseEvent.Data),
                EEventName.PlaylistNameUpdated => JsonSerializer.Deserialize<PlaylistNameUpdated>(sseEvent.Data),
                EEventName.PlaylistProtectionUpdated => JsonSerializer.Deserialize<PlaylistProtectionUpdated>(sseEvent.Data),
                EEventName.PlaylistSongsUpdated => JsonSerializer.Deserialize<PlaylistSongsUpdated>(sseEvent.Data),
                EEventName.PlaylistFavoriteByIdsUpdated => JsonSerializer.Deserialize<PlaylistFavoriteByIdsUpdated>(sseEvent.Data),
                EEventName.PlayerAvailable => JsonSerializer.Deserialize<PlayerAvailable>(sseEvent.Data),
                EEventName.PlayerRemoved => JsonSerializer.Deserialize<PlayerRemoved>(sseEvent.Data),
                EEventName.PlayerUpdated => JsonSerializer.Deserialize<PlayerUpdated>(sseEvent.Data),
                EEventName.PlayerNameUpdated => JsonSerializer.Deserialize<PlayerNameUpdated>(sseEvent.Data),
                EEventName.PlayerStateUpdated => JsonSerializer.Deserialize<PlayerStateUpdated>(sseEvent.Data),
                EEventName.PlayerIsPausedUpdated => JsonSerializer.Deserialize<PlayerIsPausedUpdated>(sseEvent.Data),
                EEventName.PlayerProtectionUpdated => JsonSerializer.Deserialize<PlayerProtectionUpdated>(sseEvent.Data),
                EEventName.PlayerCurrentSongIdUpdated => JsonSerializer.Deserialize<PlayerCurrentSongIdUpdated>(sseEvent.Data),
                EEventName.PlayerQueueEntriesDTOsUpdated => JsonSerializer.Deserialize<PlayerQueueEntriesDTOsUpdated>(sseEvent.Data),
                EEventName.PlayerQueuePositionUpdated => JsonSerializer.Deserialize<PlayerQueuePositionUpdated>(sseEvent.Data),
                EEventName.PlayerCurrentEpisodePositionUpdated => JsonSerializer.Deserialize<PlayerCurrentEpisodePositionUpdated>(sseEvent.Data),
                EEventName.PlayerNextPositionRequestUpdated => JsonSerializer.Deserialize<PlayerNextPositionRequestUpdated>(sseEvent.Data),
                EEventName.PlayerCurrentSongTimePassedUpdated => JsonSerializer.Deserialize<PlayerCurrentSongTimePassedUpdated>(sseEvent.Data),
                EEventName.PlayerCurrentSongTimeTotalUpdated => JsonSerializer.Deserialize<PlayerCurrentSongTimeTotalUpdated>(sseEvent.Data),
                EEventName.PlayerQueueIsRandomUpdated => JsonSerializer.Deserialize<PlayerQueueIsRandomUpdated>(sseEvent.Data),
                EEventName.PlayerQueueIsReversedUpdated => JsonSerializer.Deserialize<PlayerQueueIsReversedUpdated>(sseEvent.Data),
                EEventName.PlayerQueueIsNoLeftoversUpdated => JsonSerializer.Deserialize<PlayerQueueIsNoLeftoversUpdated>(sseEvent.Data),
                EEventName.PlayerQueueIsFeedRandomUpdated => JsonSerializer.Deserialize<PlayerQueueIsFeedRandomUpdated>(sseEvent.Data),
                _ => null
            };

            return pamelloEvent;
        }

        public async Task<bool> Authorize() {
            if (!IsConnected || EventsToken is null) throw new PamelloException("Events is not connected (connect to events before authorizing)");;
            if (_client.UserToken is null) throw new PamelloException("No user token (please set PamelloClient.UserToken before calling this)");

            return await _client.HttpGetAsync<Guid>($"Authorization/Events/{EventsToken}/WithToken/{_client.UserToken}") != Guid.Empty;
        }
        public async Task Unauthorize() {
            if (EventsToken is null) return;

            await _client.HttpGetAsync($"Authorization/Events/{EventsToken}/Unauthorize");
        }
        
        internal async Task Cleanup() {
            EventsToken = null;
            
            _eventsCTS.Cancel();
            _reconnectionCTS.Cancel();
            
            if (_eventStreamTask is not null) await _eventStreamTask;
            if (_reconnectionTask is not null) await _reconnectionTask;
            
            _eventsCTS = new CancellationTokenSource();
            _reconnectionCTS = new CancellationTokenSource();
        }
    }
}