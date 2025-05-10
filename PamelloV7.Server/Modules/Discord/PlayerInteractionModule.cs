using Discord.Interactions;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Attributes;

namespace PamelloV7.Server.Modules.Discord
{
    [Group("player", "Commands to interact with player")]
    public class PlayerInteractionModule : PamelloInteractionModuleBase
    {
        public PlayerInteractionModule(IServiceProvider services) : base(services) { }

        [SlashCommand("select", "Select a working player")]
        public async Task PlayerSelectHandler(
            [Summary("player", "Player id or name")] string playerValue
        ) => await PlayerSelect(playerValue);

        [SlashCommand("create", "Create new player")]
        public async Task PlayerCreateHandler(
            [Summary("name", "New player name")] string name
        ) => await PlayerCreate(name);

        [SlashCommand("protection", "Set selected player protection status")]
        public async Task PlayerProtectionHandler(
            [Summary("state", "Enable or disable protection")] EBoolState state
        ) => await PlayerProtection(state);

        [SlashCommand("info", "Get current player info")]
        public async Task PlayerInfoHandler() => await PlayerInfo();

        [SlashCommand("list", "Create new player")]
        public async Task PlayerListHandler(
            [Summary("querry", "Player list search querry")] string querry = "",
            [Summary("page", "Page of the players list")] int page = 0
        ) => await PlayerList(querry, page);

        [SlashCommand("delete", "Delete player")]
        public async Task PlayerDeleteHandler(
            [Summary("player", "Player id or name")] string playerValue
        ) => await PlayerDelete(playerValue);

        [SlashCommand("resume", "Resume music playback")]
        public async Task PlayerResumeHandler() => await PlayerResume();

        [SlashCommand("pause", "Pause music playback")]
        public async Task PlayerPauseHandler() => await PlayerPause();

        [SlashCommand("skip", "Skip current song")]
        public async Task PlayerSkipHandler() => await PlayerSkip();

        [SlashCommand("go-to", "Go to song in the queue")]
        public async Task PlayerGoToHandler(
            [Summary("song-position", "Song position in queue")] int songPosition,
			[Summary("return-back", "Return back after song ends")] bool returnBack = false
        ) => await PlayerGoTo(songPosition, returnBack);

        [SlashCommand("prev", "Go to previous song in queue")]
        public async Task PlayerPrevHandler() => await PlayerPrev();

        [SlashCommand("next", "Go to next song in queue")]
        public async Task PlayerNextHandler() => await PlayerNext();

        [SlashCommand("go-to-episode", "Rewind song to episode")]
        public async Task PlayerGoToEpisodeHandler(
            [Summary("episode-position", "Episode position in song")] int episodePosition
        ) => await PlayerGoToEpisode(episodePosition);

        [SlashCommand("next-episode", "Rewind song to previous episode")]
        public async Task PlayerNextEpisodeHandler() => await PlayerNextEpisode();

        [SlashCommand("prev-episode", "Rewind song to next episode")]
        public async Task PlayerPrevEpisodeHandler() => await PlayerPrevEpisode();

        [SlashCommand("rewind", "Rewind song", runMode: RunMode.Async)]
        public async Task PlayerRewindHandler(
            [Summary("time", "Time in format HH:MM:SS")] string time
        ) => await PlayerRewind(time);

        [Group("queue", "Commands to interact with selected player queue")]
        public class PlayerQueueInteractionModule : PamelloInteractionModuleBase {
            public PlayerQueueInteractionModule(IServiceProvider services) : base(services) { } 

            [SlashCommand("add-song", "Add song to the queue")]
            public async Task PlayerQueueSongAddHandler(
                [Summary("song", "Song id/association/name/youtube-url")] string songValue
            ) => await PlayerQueueSongAdd(songValue);

            [SlashCommand("insert-song", "Add playlist to the queue")]
            public async Task PlayerQueueSongInsertHandler(
                [Summary("position", "Where song should be inserted")] int position,
                [Summary("song", "Song id/association/name/youtube-url")] string songValue
            ) => await PlayerQueueSongInsert(position, songValue);

            [SlashCommand("add-playlist", "Add playlist to the queue")]
            public async Task PlayerQueuePlaylistAddHandler(
                [Summary("playlist", "Playlist id or name")] string playlistValue
            ) => await PlayerQueuePlaylistAdd(playlistValue);
            
            [SlashCommand("insert-playlist", "Insert playlist to the queue")]
            public async Task PlayerQueuePlaylistInsertHandler(
                [Summary("position", "Where song should be inserted")] int position,
                [Summary("playlist", "Playlist id or name")] string playlistValue
            ) => await PlayerQueuePlaylistInsert(position, playlistValue);

            [SlashCommand("remove", "Remove song from the queue")]
            public async Task PlayerQueueSongRemoveHandler(
                [Summary("position", "Position of song that should be removed")] int position
            ) => await PlayerQueueSongRemove(position);

            [SlashCommand("song-move", "Move song from one place to another")]
            public async Task PlayerQueueSongMoveHandler(
                [Summary("from-position", "Position of the song that should be moved")] int fromPosition,
                [Summary("to-position", "Position to which the song should be moved")] int toPosition
            ) => await PlayerQueueSongMove(fromPosition, toPosition);

            [SlashCommand("song-swap", "Swap one song with another")]
            public async Task PlayerQueueSongSwapHandler(
                [Summary("in-position", "Position of song that should be swaped with another song")] int inPosition,
				[Summary("with-position", "Position of another song")] int withPosition
            ) => await PlayerQueueSongSwap(inPosition, withPosition);

            [SlashCommand("song-request-next", "Request the song to be played next")]
            public async Task PlayerQueueSongRequestNextHandler(
                [Summary("position", "Position of the song")] int position
            ) => await PlayerQueueSongRequestNext(position);

            [SlashCommand("random", "Toggle random playback of queue songs")]
            public async Task PlayerQueueRandomHandler(
                [Summary("state", "Enable or disable mode")] EBoolState state
            ) => await PlayerQueueRandom(state);

            [SlashCommand("reversed", "Toggle reversed playback of queue songs")]
            public async Task PlayerQueueReversedHandler(
                [Summary("state", "Enable or disable mode")] EBoolState state
            ) => await PlayerQueueReversed(state);

            [SlashCommand("no-leftovers", "Toggle deletion of played songs from queue")]
            public async Task PlayerQueueNoLeftoversHandler(
                [Summary("state", "Enable or disable mode")] EBoolState state
            ) => await PlayerQueueNoLeftovers(state);

            [SlashCommand("feed-random", "Toggle deletion of played songs from queue")]
            public async Task PlayerQueueFeedRandomHandler(
                [Summary("state", "Enable or disable mode")] EBoolState state
            ) => await PlayerQueueFeedRandom(state);

            [SlashCommand("list", "Get queue songs list")]
            public async Task PlayerQueueSuffleHandler(
                [Summary("page", "Page of the songs list")] int? page = 1
            ) => await PlayerQueueList(page);

            public async Task PlayerQueueSuffleHandler(
                
            ) => await PlayerQueueSuffle();

            [SlashCommand("clear", "Clear queue")]
            public async Task PlayerQueueClearHandler() => await PlayerQueueClear();
        }
    }
}
