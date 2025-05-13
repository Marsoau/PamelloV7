using Discord.Interactions;
using Discord.WebSocket;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Modules.Discord.Base;

namespace PamelloV7.Server.Modules.Discord
{
    [Group("playlist", "Commands to interact with playlists")]
    public class PlaylistInteractionModule : PamelloInteractionModuleBase
    {
        public PlaylistInteractionModule(IServiceProvider services) : base(services) { }

        [SlashCommand("create", "Create new playlist")]
        public async Task PlaylistCreateHandler(
            [Summary("name", "Playlist name")] string name,
            [Summary("fill-with-queue", "Fill new playlist with songs currently in queue")] EBoolAnswer fillWithQueue = EBoolAnswer.No
        ) => await PlaylistCreate(name, fillWithQueue == EBoolAnswer.Yes);

        [SlashCommand("add-song", "Add song to the playlist")]
        public async Task PlaylistAddSongHandler(
            [Summary("playlist", "Playlist id/name")] string playlistValue,
            [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current",
            [Summary("position", "Position where song will be added")] int? position = null
        ) => await PlaylistAddSong(playlistValue, songValue, position);

        [SlashCommand("add-playlist", "Add playlist songs to another playlist")]
        public async Task PlaylistAddPlaylistSongsHandler(
            [Summary("to-playlist", "Playlist id/name")] string toPlaylistValue,
            [Summary("from-playlist", "Playlist id/name")] string fromPlaylistValue,
            [Summary("position", "Position where playlist songs will be added")] int? position = null
        ) => await PlaylistAddPlaylistSongs(toPlaylistValue, fromPlaylistValue, position);

        [SlashCommand("remove-song", "Remove all copies of song from playlist")]
        public async Task PlaylistRemoveSongHandler(
            [Summary("playlist", "Playlist id/name")] string playlistValue,
            [Summary("song", "Song id/association/name/youtube-url")] string songValue = "current"
        ) => await PlaylistRemoveSong(playlistValue, songValue);
        
        [SlashCommand("remove-at", "Show playlist info")]
        public async Task PlaylistRemoveAtHandler(
            [Summary("playlist", "Playlist id/name")] string playlistValue,
            [Summary("position", "Position where song will be removed")] int position
        ) => await PlaylistRemoveAt(playlistValue, position);
        
        [SlashCommand("move-song", "Add playlist songs to another playlist")]
        public async Task PlaylistMoveSongHandler(
            [Summary("playlist", "Playlist id/name")] string playlistValue,
            [Summary("from-position", "Position of the song that should be moved")] int fromPosition,
            [Summary("to-position", "To where song should be moved")] int toPosition
        ) => await PlaylistMoveSong(playlistValue, fromPosition, toPosition);

        [SlashCommand("search", "Search playlists in the database")]
        public async Task PlaylistSearchHandler(
            [Summary("querry", "Playlist name querry")] string querry = "",
            [Summary("page", "Results page")] int page = 1,
            [Summary("added-by", "Filter by playlists that were added by specified user")] SocketUser? addedByDiscordUser = null
        ) => await PlaylistSearch(querry, page, addedByDiscordUser);

        [SlashCommand("songs-list", "Show all songs in playlist")]
        public async Task PlaylistSongsListHandler(
            [Summary("playlist", "Playlist id/name")] string playlistValue,
            [Summary("page", "Results page")] int page = 1
        ) => await PlaylistSongsList(playlistValue, page);

        [SlashCommand("info", "Show playlist info")]
        public async Task PlaylistInfoHandler(
            [Summary("playlist", "Playlist id/name")] string playlistValue
        ) => await PlaylistInfo(playlistValue);

        [SlashCommand("rename", "Rename playlist")]
        public async Task PlaylistRenameHandler(
            [Summary("new-name", "Playlist new name")] string name,
            [Summary("playlist", "Playlist id/name")] string playlistValue
        ) => await PlaylistRename(playlistValue, name);

        [Group("favorite", "Commands to interact with playlists")]
        public class PlaylistFavoriteInteractionModule : PamelloInteractionModuleBase
        {
            public PlaylistFavoriteInteractionModule(IServiceProvider services) : base(services) { }

            [SlashCommand("add", "Rename playlist")]
            public async Task PlaylistFavoriteAddHandler(
                [Summary("playlist", "Playlist id/name")] string playlistValue
            ) => await PlaylistFavoriteAdd(playlistValue);

            [SlashCommand("remove", "Rename playlist")]
            public async Task PlaylistFavoriteRemoveHandler(
                [Summary("playlist", "Playlist id/name")] string playlistValue
            ) => await PlaylistFavoriteRemove(playlistValue);

            [SlashCommand("list", "Rename playlist")]
            public async Task PlaylistFavoriteListHandler(
                [Summary("querry", "Song name querry")] string querry = "",
                [Summary("page", "Results page")] int page = 1,
                [Summary("favorite-by", "Get favorite songs of other user")] SocketUser? targetDiscordUser = null
            ) => await PlaylistFavoriteList(querry, page, targetDiscordUser);
        }

        [SlashCommand("delete", "Show playlist info")]
        public async Task PlaylistDeleteHandler(
            [Summary("playlist", "Playlist id/name")] string playlistValue
        ) => await PlaylistDelete(playlistValue);
    }
}
