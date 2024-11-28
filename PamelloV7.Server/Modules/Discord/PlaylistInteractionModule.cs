using Discord.Interactions;

namespace PamelloV7.Server.Modules.Discord
{
    [Group("playlist", "Commands to interact with playlists")]
    public class PlaylistInteractionModule : PamelloInteractionModuleBase
    {
        public PlaylistInteractionModule(IServiceProvider services) : base(services) { }

        public async Task PlaylistCreateHandler()
            => await PlaylistCreate("", false);
        public async Task PlaylistAddSongHandler()
            => await PlaylistAddSong("", "");
        public async Task PlaylistAddPlaylistSongsHandler()
            => await PlaylistAddPlaylistSongs();
        public async Task PlaylistSearchHandler()
            => await PlaylistSearch();
        public async Task PlaylistInfoHandler()
            => await PlaylistInfo();
        public async Task PlaylistRenameHandler()
            => await PlaylistRename();

        [Group("favorite", "Commands to interact with playlists")]
        public class PlaylistFavoriteInteractionModule : PamelloInteractionModuleBase
        {
            public PlaylistFavoriteInteractionModule(IServiceProvider services) : base(services) { }

            public async Task PlaylistFavoriteAddHandler()
                => await PlaylistFavoriteAdd();
            public async Task PlaylistFavoriteRemoveHandler()
                => await PlaylistFavoriteRemove();
            public async Task PlaylistFavoriteListHandler()
                => await PlaylistFavoriteList();
        }
        public async Task PlaylistDeleteHandler()
            => await PlaylistDelete();
    }
}
