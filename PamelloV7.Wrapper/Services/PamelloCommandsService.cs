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
        public async Task<TReturnType?> InvokeCommand<TReturnType>(string commandStr) {
            try {
                return await _client.HttpGetAsync<TReturnType>($"Command/{commandStr}");
            }
            catch (Exception x) {
                Console.WriteLine($"Exceprion in command \"{commandStr}\": {x}");
            }

            return default;
        }

        public async Task<int> PlayerCreate(string playerName) {
            return await InvokeCommand<int>($"PlayerCreate?playerName={playerName}");
        }
        public async Task PlayerSelect(string? playerValue) {
            if (playerValue is null) {
                await InvokeCommand($"PlayerSelect");
            }
            else {
                await InvokeCommand($"PlayerSelect?player={playerValue}");
            }
        }
        public async Task PlayerProtection(bool state) {
            await InvokeCommand($"PlayerProtection?state={state}");
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
        public async Task PlayerGoToEpisode(int episodePosition) {
            await InvokeCommand($"PlayerGoToEpisode?episodePosition={episodePosition}");
        }
        public async Task PlayerPrevEpisode() {
            await InvokeCommand($"PlayerPrevEpisode");
        }
        public async Task PlayerNextEpisode() {
            await InvokeCommand($"PlayerNextEpisode");
        }
        public async Task PlayerRewind(int seconds) {
            await InvokeCommand($"PlayerRewind?seconds={seconds}");
        }
        public async Task PlayerQueueSongAdd(string songValue) {
            await InvokeCommand($"PlayerQueueSongAdd?song={songValue}");
        }
        public async Task PlayerQueueSongInsert(int queuePosition, string songValue) {
            await InvokeCommand($"PlayerQueueSongInsert?queuePosition={queuePosition}&song={songValue}");
        }
        public async Task PlayerQueueSongYoutubeAdd(string youtubeId) {
            await InvokeCommand($"PlayerQueueSongYoutubeAdd?youtubeId={youtubeId}");
        }
        public async Task PlayerQueueSongYoutubeInsert(int queuePosition, string youtubeId) {
            await InvokeCommand($"PlayerQueueSongYoutubeInsert?queuePosition={queuePosition}&youtubeId={youtubeId}");
        }
        public async Task PlayerQueuePlaylistAdd(string playlistValue) {
            await InvokeCommand($"PlayerQueuePlaylistAdd?playlist={playlistValue}");
        }
        public async Task PlayerQueuePlaylistInsert(int queuePosition, string playlistValue) {
            await InvokeCommand($"PlayerQueuePlaylistInsert?queuePosition={queuePosition}&playlist={playlistValue}");
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
        public async Task PlayerQueueSuffle() {
            await InvokeCommand($"PlayerQueueSuffle");
        }
        public async Task PlayerQueueClear() {
            await InvokeCommand($"PlayerQueueClear");
        }
        public async Task<int> SongAdd(string youtubeId) {
            return await InvokeCommand<int>($"SongAdd?youtubeId={youtubeId}");
        }
        public async Task SongRename(string songValue, string newName) {
            await InvokeCommand($"SongRename?song={songValue}&newName={newName}");
        }
        public async Task SongFavoriteAdd(string songValue) {
            await InvokeCommand($"SongFavoriteAdd?song={songValue}");
        }
        public async Task SongFavoriteRemove(string songValue) {
            await InvokeCommand($"SongFavoriteRemove?song={songValue}");
        }
        public async Task SongAssociacionsAdd(string songValue, string associacion) {
            await InvokeCommand($"SongAssociacionsAdd?song={songValue}&associacion={associacion}");
        }
        public async Task SongAssociacionsRemove(string songValue, string associacion) {
            await InvokeCommand($"SongAssociacionsRemove?song={songValue}&associacion={associacion}");
        }
        public async Task<int> SongEpisodesAdd(string songValue, int episodeStart, string episodeName) {
            return await InvokeCommand<int>($"SongEpisodesAdd?song={songValue}&episodeStart={episodeStart}&episodeName={episodeName}");
        }
        public async Task SongEpisodesRemove(string songValue, int episodePosition) {
            await InvokeCommand($"SongEpisodesRemove?song={songValue}&episodePosition={episodePosition}");
        }
        public async Task SongEpisodesRename(string songValue, int episodePosition, string newName) {
            await InvokeCommand($"SongEpisodesRename?song={songValue}&episodePosition={episodePosition}&newName={newName}");
        }
        public async Task SongEpisodesSkipSet(string songValue, int episodePosition, bool newState) {
            await InvokeCommand($"SongEpisodesSkipSet?song={songValue}&episodePosition={episodePosition}&newState={newState}");
        }
        public async Task SongEpisodesEditTime(string songValue, int episodePosition, int newTime) {
            await InvokeCommand($"SongEpisodesEditTime?song={songValue}&episodePosition={episodePosition}&newTime={newTime}");
        }
        public async Task SongEpisodesClear(string songValue) {
            await InvokeCommand($"SongEpisodesClear?song={songValue}");
        }
        public async Task<int> PlaylistCreate(string name, bool fillWithQueue) {
            return await InvokeCommand<int>($"PlaylistCreate?name={name}&fillWithQueue={fillWithQueue}");
        }
        public async Task PlaylistAddSong(string playlistValue, string songValue) {
            await InvokeCommand($"PlaylistAddSong?playlist={playlistValue}&song={songValue}");
        }
        public async Task<int> PlaylistAddPlaylistSongs(int toPlaylistId, int fromPlaylistId) {
            return await InvokeCommand<int>($"PlaylistAddPlaylistSongs?toPlaylistId={toPlaylistId}&fromPlaylistId={fromPlaylistId}");
        }
        public async Task PlaylistRemoveSong(string playlistValue, string songValue) {
            await InvokeCommand($"PlaylistRemoveSong?playlist={playlistValue}&song={songValue}");
        }
        public async Task PlaylistRename(string playlistValue, string newName) {
            await InvokeCommand($"PlaylistRename?playlist={playlistValue}&newName={newName}");
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
        public async Task SpeakerConnect() {
            await InvokeCommand($"SpeakerConnect");
        }
        public async Task SpeakerDisconnect() {
            await InvokeCommand($"SpeakerDisconnect");
        }
    }
}
