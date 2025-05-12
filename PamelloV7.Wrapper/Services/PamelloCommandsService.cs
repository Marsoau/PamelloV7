using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Console.WriteLine($"Exceprion in command \"{commandStr}\": {x}");
            }
        }
        public async Task<TReturnType> InvokeCommand<TReturnType>(string commandStr) {
            return await _client.HttpGetAsync<TReturnType>($"Command/{commandStr}");
        }

        public async Task<int> PlayerCreate(string playerName) {
            return await InvokeCommand<int>($"PlayerCreate?playerName={playerName}");
        }
        public async Task<int?> PlayerSelect(string? playerValue) {
            if (playerValue is null) {
                return await InvokeCommand<int?>($"PlayerSelect");
            } {
                return await InvokeCommand<int?>($"PlayerSelect?player={playerValue}");
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
        public async Task<int?> PlayerSkip() {
            return await InvokeCommand<int?>($"PlayerSkip");
        }
        public async Task<int> PlayerGoTo(int songPosition, bool returnBack) {
            return await InvokeCommand<int>($"PlayerGoTo?songPosition={songPosition}&returnBack={returnBack}");
        }
        public async Task<int> PlayerPrev() {
            return await InvokeCommand<int>($"PlayerPrev");
        }
        public async Task<int> PlayerNext() {
            return await InvokeCommand<int>($"PlayerNext");
        }
        public async Task<int?> PlayerGoToEpisode(int episodePosition) {
            return await InvokeCommand<int?>($"PlayerGoToEpisode?episodePosition={episodePosition}");
        }
        public async Task<int?> PlayerPrevEpisode() {
            return await InvokeCommand<int?>($"PlayerPrevEpisode");
        }
        public async Task<int?> PlayerNextEpisode() {
            return await InvokeCommand<int?>($"PlayerNextEpisode");
        }
        public async Task PlayerRewind(int seconds) {
            await InvokeCommand($"PlayerRewind?seconds={seconds}");
        }
        public async Task<int> PlayerQueueSongAdd(string songValue) {
            return await InvokeCommand<int>($"PlayerQueueSongAdd?song={songValue}");
        }
        public async Task<int> PlayerQueueSongInsert(int queuePosition, string songValue) {
            return await InvokeCommand<int>($"PlayerQueueSongInsert?queuePosition={queuePosition}&song={songValue}");
        }
        public async Task<int> PlayerQueuePlaylistAdd(string playlistValue) {
            return await InvokeCommand<int>($"PlayerQueuePlaylistAdd?playlist={playlistValue}");
        }
        public async Task<int> PlayerQueuePlaylistInsert(int queuePosition, string playlistValue) {
            return await InvokeCommand<int>($"PlayerQueuePlaylistInsert?queuePosition={queuePosition}&playlist={playlistValue}");
        }
        public async Task<int> PlayerQueueSongRemove(int position) {
            return await InvokeCommand<int>($"PlayerQueueSongRemove?position={position}");
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
        public async Task<int> SongAdd(string youtubeId) {
            return await InvokeCommand<int>($"SongAdd?youtubeId={youtubeId}");
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
        public async Task SongEpisodeClear(string songValue) {
            await InvokeCommand($"SongEpisodeClear?song={songValue}");
        }
        public async Task<int> PlaylistCreate(string name, bool fillWithQueue) {
            return await InvokeCommand<int>($"PlaylistCreate?name={name}&fillWithQueue={fillWithQueue}");
        }
        public async Task<int?> PlaylistAddSong(string playlistValue, string songValue) {
            return await InvokeCommand<int?>($"PlaylistAddSong?playlist={playlistValue}&song={songValue}");
        }
        public async Task<int> PlaylistAddPlaylistSongs(int toPlaylistId, int fromPlaylistId) {
            return await InvokeCommand<int>($"PlaylistAddPlaylistSongs?toPlaylistId={toPlaylistId}&fromPlaylistId={fromPlaylistId}");
        }
        public async Task<int?> PlaylistRemoveSong(string playlistValue, string songValue) {
            return await InvokeCommand<int?>($"PlaylistRemoveSong?playlist={playlistValue}&song={songValue}");
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
        public async Task SpeakerDisconnect() {
            await InvokeCommand($"SpeakerDisconnect");
        }
        public async Task<string> SpeakerInternetConnect(string? channel = null, bool isPublic = false) {
            return await InvokeCommand<string>($"SpeakerInternetConnect?channel={channel}&isPublic={isPublic}");
        }
        public async Task<bool> SpeakerInternetChangeProtection(string channel, bool isPublic = false) {
            return await InvokeCommand<bool>($"SpeakerInternetChangeProtection?channel={channel}&isPublic={isPublic}");
        }
    }
}
