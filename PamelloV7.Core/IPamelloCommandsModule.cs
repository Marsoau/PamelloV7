using PamelloV7.Core.AudioOld;
using System;
using System.Threading.Tasks;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core
{
    public interface IPamelloCommandsModule
    {
        IPamelloUser User { get; }

        // Player commands
        Task<IPamelloPlayer> PlayerCreate(string playerName);
        Task<IPamelloPlayer?> PlayerSelect(IPamelloPlayer? player);
        Task<bool> PlayerProtection(bool state);
        Task PlayerDelete();
        Task PlayerResume();
        Task PlayerPause();
        Task<IPamelloSong?> PlayerSkip();
        Task<IPamelloSong> PlayerGoTo(int songPosition, bool returnBack);
        Task<IPamelloSong> PlayerPrev();
        Task<IPamelloSong> PlayerNext();
        Task<IPamelloEpisode?> PlayerGoToEpisode(int episodePosition);
        Task<IPamelloEpisode?> PlayerPrevEpisode();
        Task<IPamelloEpisode?> PlayerNextEpisode();
        Task PlayerRewind(int seconds);
        Task<IPamelloSong> PlayerQueueSongAdd(IPamelloSong song, int? position = null);
        Task<IPamelloPlaylist> PlayerQueuePlaylistAdd(IPamelloPlaylist playlist, int? position = null);
        Task<IPamelloSong> PlayerQueueSongRemove(int position);
        Task PlayerQueueSongSwap(int inPosition, int withPosition);
        Task PlayerQueueSongMove(int fromPosition, int toPosition);
        Task PlayerQueueSongRequestNext(int? position);
        Task PlayerQueueRandom(bool value);
        Task PlayerQueueReversed(bool value);
        Task PlayerQueueNoLeftovers(bool value);
        Task PlayerQueueFeedRandom(bool value);
        Task PlayerQueueShuffle();
        Task PlayerQueueClear();

        // Song commands
        Task<IPamelloSong> SongAdd(string youtubeId);
        Task<string> SongRename(IPamelloSong song, string newName);
        Task SongFavoriteAdd(IPamelloSong song);
        Task SongFavoriteRemove(IPamelloSong song);
        Task SongAssociationsAdd(IPamelloSong song, string associacion);
        Task SongAssociationsRemove(IPamelloSong song, string associacion);
        Task<IPamelloEpisode> SongEpisodeAdd(IPamelloSong song, int episodeStart, string episodeName);
        Task SongEpisodeRemove(IPamelloSong song, int episodePosition);
        Task<string> SongEpisodeRename(IPamelloSong song, int episodePosition, string newName);
        Task SongEpisodeSkipSet(IPamelloSong song, int episodePosition, bool newState);
        Task<int> SongEpisodeEditTime(IPamelloSong song, int episodePosition, int newTime);
        Task SongEpisodesClear(IPamelloSong song);

        // Playlist commands
        Task<IPamelloPlaylist> PlaylistCreate(string name, bool fillWithQueue);
        Task<IPamelloSong?> PlaylistAddSong(IPamelloPlaylist playlist, IPamelloSong song, int? position = null);
        Task PlaylistAddPlaylistSongs(IPamelloPlaylist fromPlaylist, IPamelloPlaylist toPlaylist, int? position = null);
        Task<int> PlaylistRemoveSong(IPamelloPlaylist playlist, IPamelloSong song);
        Task<IPamelloSong?> PlaylistRemoveAt(IPamelloPlaylist playlist, int position);
        Task<IPamelloSong?> PlaylistMoveSong(IPamelloPlaylist playlist, int fromPosition, int toPosition);
        Task<string> PlaylistRename(IPamelloPlaylist playlist, string newName);
        Task PlaylistFavoriteAdd(IPamelloPlaylist playlist);
        Task PlaylistFavoriteRemove(IPamelloPlaylist playlist);
        Task PlaylistDelete(IPamelloPlaylist playlist);

        // Speaker commands
        Task<IPamelloInternetSpeaker> SpeakerInternetConnect(string? name);
        Task SpeakerDisconnect(IPamelloSpeaker speaker);
        Task<string> SpeakerInternetRename(IPamelloInternetSpeaker speaker, string newName);
    }
}
