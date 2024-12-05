using Discord.Interactions;
using Discord.WebSocket;
using PamelloV7.Core.Enumerators;

namespace PamelloV7.Server.Modules.Discord
{
    [Group("playlist", "Commands to interact with playlists")]
    public class PlaylistInteractionModule : PamelloInteractionModuleBase
    {
        public PlaylistInteractionModule(IServiceProvider services) : base(services) { }

        [SlashCommand("create", "Create new playlist")]
        public async Task PlaylistCreateHandler(
            [Summary("name", "Playlist name")] string name,
            [Summary("fill-with-queue", "Fill new playlist with songs currently in queue")] EBoolState fillWithQueue = EBoolState.Disabled
        ) => await PlaylistCreate(name, fillWithQueue == EBoolState.Enabled);

        [SlashCommand("add-song", "Add song to the playlist")]
        public async Task PlaylistAddSongHandler(
            [Summary("playlist", "Playlist id/name")] string playlistValue,
            [Summary("song", "Song id/associacion/name/youtube-url")] string songValue
        ) => await PlaylistAddSong(playlistValue, songValue);

        [SlashCommand("add-playlist", "Add playlist songs to another playlist")]
        public async Task PlaylistAddPlaylistSongsHandler(
            [Summary("to-playlist", "Playlist id/name")] string toPlaylistValue,
            [Summary("from-playlist", "Playlist id/name")] string fromPlaylistValue
        ) => await PlaylistAddPlaylistSongs(toPlaylistValue, fromPlaylistValue);

        [SlashCommand("remove-song", "Show playlist info")]
        public async Task PlaylistRemoveSongHandler(
            [Summary("playlist", "Playlist id/name")] string playlistValue,
            [Summary("song", "Song id/associacion/name/youtube-url")] string songValue
        ) => await PlaylistRemoveSong(playlistValue, songValue);

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
            [Summary("playlist", "Playlist id/name")] string playlistValue,
            [Summary("new-name", "Playlist new name")] string name
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
