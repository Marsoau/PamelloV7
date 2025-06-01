using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PamelloV7.Wrapper.Interfaces;
using PamelloV7.Wrapper.Model;

namespace PamelloV7.Wrapper.Services
{
    public class PamelloCommandsService
    {
        private readonly PamelloClient _client;

        public PamelloCommandsService(PamelloClient client) {
            _client = client;
        }

        public async Task InvokeCommand(string commandStr) {
            try {
                await _client.HttpGetAsync<object>($"Command/{commandStr}");
            }
            catch (Exception x) {
                _client.Log(($"Exceprion in command \"{commandStr}\": {x}"));
            }
        }
        public async Task<TReturnType> InvokeCommand<TReturnType>(string commandStr) {
            if (typeof(TReturnType).IsAssignableTo(typeof(IRemoteEntity))) {
                var entityId = await InvokeCommand<int>($"Command/{commandStr}");
                
                if (typeof(TReturnType) == typeof(RemoteUser)) {
                    return (TReturnType)(object)await _client.Users.GetRequired(entityId);
                }
                if (typeof(TReturnType) == typeof(RemoteSong)) {
                    return (TReturnType)(object)await _client.Songs.GetRequired(entityId);
                }
                if (typeof(TReturnType) == typeof(RemoteEpisode)) {
                    return (TReturnType)(object)await _client.Episodes.GetRequired(entityId);
                }
                if (typeof(TReturnType) == typeof(RemotePlaylist)) {
                    return (TReturnType)(object)await _client.Playlists.GetRequired(entityId);
                }
                if (typeof(TReturnType) == typeof(RemotePlayer)) {
                    return (TReturnType)(object)await _client.Players.GetRequired(entityId);
                }
            }
            
            return await _client.HttpGetAsync<TReturnType>($"Command/{commandStr}");
        }

        public async Task<int> PlayerCreate(string playerName) {
            return await InvokeCommand<int>($"PlayerCreate?playerName={playerName}");
        }
        public async Task<RemotePlayer?> PlayerSelect(string? playerValue) {
            if (playerValue is null) {
                return await InvokeCommand<RemotePlayer?>($"PlayerSelect");
            } {
                return await InvokeCommand<RemotePlayer?>($"PlayerSelect?player={playerValue}");
            }
        }
        public async Task<bool> PlayerProtection(bool state) {
            return await InvokeCommand<bool>($"PlayerProtection?state={state}");
        }
        public async Task PlayerDelete() {
            await InvokeCommand($"PlayerDelete");
        }
        public async Task PlayerResume() {
            await InvokeCommand($"PlayerResume");
        }
        public async Task PlayerPause() {
            await InvokeCommand($"PlayerPause");
        }
        public async Task<RemoteSong?> PlayerSkip() {
            return await InvokeCommand<RemoteSong?>($"PlayerSkip");
        }
        public async Task<RemoteSong> PlayerGoTo(int songPosition, bool returnBack) {
            return await InvokeCommand<RemoteSong>($"PlayerGoTo?songPosition={songPosition}&returnBack={returnBack}");
        }
        public async Task<RemoteSong> PlayerPrev() {
            return await InvokeCommand<RemoteSong>($"PlayerPrev");
        }
        public async Task<RemoteSong> PlayerNext() {
            return await InvokeCommand<RemoteSong>($"PlayerNext");
        }
        public async Task<RemoteEpisode?> PlayerGoToEpisode(int episodePosition) {
            return await InvokeCommand<RemoteEpisode?>($"PlayerGoToEpisode?episodePosition={episodePosition}");
        }
        public async Task<RemoteEpisode?> PlayerPrevEpisode() {
            return await InvokeCommand<RemoteEpisode?>($"PlayerPrevEpisode");
        }
        public async Task<RemoteEpisode?> PlayerNextEpisode() {
            return await InvokeCommand<RemoteEpisode?>($"PlayerNextEpisode");
        }
        public async Task PlayerRewind(int seconds) {
            await InvokeCommand($"PlayerRewind?seconds={seconds}");
        }
        public async Task<RemoteSong> PlayerQueueSongAdd(string songValue, int? position = null) {
            return await InvokeCommand<RemoteSong>($"PlayerQueueSongAdd?song={songValue}&position={position}");
        }
        public async Task<RemotePlaylist> PlayerQueuePlaylistAdd(string playlistValue, int? position = null) {
            return await InvokeCommand<RemotePlaylist>($"PlayerQueuePlaylistAdd?playlist={playlistValue}&position={position}");
        }
        public async Task<RemoteSong> PlayerQueueSongRemove(int position) {
            return await InvokeCommand<RemoteSong>($"PlayerQueueSongRemove?position={position}");
        }
        public async Task PlayerQueueSongSwap(int inPosition, int withPosition) {
            await InvokeCommand($"PlayerQueueSongSwap?inPosition={inPosition}&withPosition={withPosition}");
        }
        public async Task PlayerQueueSongMove(int fromPosition, int toPosition) {
            await InvokeCommand($"PlayerQueueSongMove?fromPosition={fromPosition}&toPosition={toPosition}");
        }
        public async Task PlayerQueueSongRequestNext(int? position) {
            await InvokeCommand($"PlayerQueueSongRequestNext?position={position}");
        }
        public async Task PlayerQueueRandom(bool value) {
            await InvokeCommand($"PlayerQueueRandom?value={value}");
        }
        public async Task PlayerQueueReversed(bool value) {
            await InvokeCommand($"PlayerQueueReversed?value={value}");
        }
        public async Task PlayerQueueNoLeftovers(bool value) {
            await InvokeCommand($"PlayerQueueNoLeftovers?value={value}");
        }
        public async Task PlayerQueueFeedRandom(bool value) {
            await InvokeCommand($"PlayerQueueFeedRandom?value={value}");
        }
        public async Task PlayerQueueShuffle() {
            await InvokeCommand($"PlayerQueueShuffle");
        }
        public async Task PlayerQueueClear() {
            await InvokeCommand($"PlayerQueueClear");
        }
        public async Task<RemoteSong> SongAdd(string youtubeId) {
            return await InvokeCommand<RemoteSong>($"SongAdd?youtubeId={youtubeId}");
        }
        public Task<string> SongRename(string songValue, string newName) {
            return InvokeCommand<string>($"SongRename?song={songValue}&newName={newName}");
        }
        public async Task SongFavoriteAdd(string songValue) {
            await InvokeCommand($"SongFavoriteAdd?song={songValue}");
        }
        public async Task SongFavoriteRemove(string songValue) {
            await InvokeCommand($"SongFavoriteRemove?song={songValue}");
        }
        public async Task SongAssociationsAdd(string songValue, string associacion) {
            await InvokeCommand($"SongAssociationsAdd?song={songValue}&associacion={associacion}");
        }
        public async Task SongAssociationsRemove(string songValue, string associacion) {
            await InvokeCommand($"SongAssociationsRemove?song={songValue}&associacion={associacion}");
        }
        public async Task<int> SongEpisodeAdd(string songValue, int episodeStart, string episodeName) {
            return await InvokeCommand<int>($"SongEpisodeAdd?song={songValue}&episodeStart={episodeStart}&episodeName={episodeName}");
        }
        public async Task SongEpisodeRemove(string songValue, int episodePosition) {
            await InvokeCommand($"SongEpisodeRemove?song={songValue}&episodePosition={episodePosition}");
        }
        public async Task<string> SongEpisodeRename(string songValue, int episodePosition, string newName) {
            return await InvokeCommand<string>($"SongEpisodeRename?song={songValue}&episodePosition={episodePosition}&newName={newName}");
        }
        public async Task SongEpisodeSkipSet(string songValue, int episodePosition, bool newState) {
            await InvokeCommand($"SongEpisodeSkipSet?song={songValue}&episodePosition={episodePosition}&newState={newState}");
        }
        public async Task<int> SongEpisodeEditTime(string songValue, int episodePosition, int newTime) {
            return await InvokeCommand<int>($"SongEpisodeEditTime?song={songValue}&episodePosition={episodePosition}&newTime={newTime}");
        }
        public async Task SongEpisodesClear(string songValue) {
            await InvokeCommand($"SongEpisodesClear?song={songValue}");
        }
        public async Task<RemotePlaylist> PlaylistCreate(string name, bool fillWithQueue) {
            return await InvokeCommand<RemotePlaylist>($"PlaylistCreate?name={name}&fillWithQueue={fillWithQueue}");
        }
        public async Task<RemoteSong?> PlaylistAddSong(string playlistValue, string songValue, int? position = null) {
            return await InvokeCommand<RemoteSong?>($"PlaylistAddSong?playlist={playlistValue}&song={songValue}&position={position}");
        }
        public async Task PlaylistAddPlaylistSongs(string toPlaylistValue, string fromPlaylistValue, int? position = null) {
            await InvokeCommand($"PlaylistAddPlaylistSongs?toPlaylist={toPlaylistValue}&fromPlaylist={fromPlaylistValue}&position={position}");
        }
        public async Task<RemoteSong?> PlaylistRemoveSong(string playlistValue, string songValue) {
            return await InvokeCommand<RemoteSong?>($"PlaylistRemoveSong?playlist={playlistValue}&song={songValue}");
        }
        public async Task<RemoteSong?> PlaylistRemoveAt(string playlistValue, int position) {
            return await InvokeCommand<RemoteSong?>($"PlaylistRemoveAt?playlist={playlistValue}&position={position}");
        }
        public async Task<RemoteSong?> PlaylistMoveSong(string playlistValue, int fromPosition, int toPosition) {
            return await InvokeCommand<RemoteSong?>($"PlaylistMoveSong?playlist={playlistValue}&fromPosition={fromPosition}&toPosition={toPosition}");
        }
        public async Task<string> PlaylistRename(string playlistValue, string newName) {
            return await InvokeCommand<string>($"PlaylistRename?playlist={playlistValue}&newName={newName}");
        }
        public async Task PlaylistFavoriteAdd(string playlistValue) {
            await InvokeCommand($"PlaylistFavoriteAdd?playlist={playlistValue}");
        }
        public async Task PlaylistFavoriteRemove(string playlistValue) {
            await InvokeCommand($"PlaylistFavoriteRemove?playlist={playlistValue}");
        }
        public async Task PlaylistDelete(string playlistValue) {
            await InvokeCommand($"PlaylistDelete?playlist={playlistValue}");
        }
        
        public async Task SpeakerDiscordConnect() {
            await InvokeCommand($"SpeakerDiscordConnect");
        }
        public async Task SpeakerDisconnect(string speakerValue) {
            await InvokeCommand($"SpeakerDisconnect?speaker={speakerValue}");;
        }
        public async Task<string> SpeakerInternetConnect(string? name = null) {
            return await InvokeCommand<string>($"SpeakerInternetConnect?name={name}");
        }
        public async Task<bool> SpeakerInternetRename(string speakerValue, string newName) {
            return await InvokeCommand<bool>($"SpeakerInternetChangeProtection?speaker={speakerValue}&newName={newName}");
        }
    }
}
