using Discord.Interactions;

namespace PamelloV7.Server.Modules.Discord
{
    [Group("song", "Commands to interact with songs")]
    public class SongInteractionModule : PamelloInteractionModuleBase
    {
        public SongInteractionModule(IServiceProvider services) : base(services) { }

        public async Task SongAddHandler()
            => await SongAdd();
        public async Task SongSearchHandler()
            => await SongSearch();
        public async Task SongInfoHandler()
            => await SongInfo();
        public async Task SongRenameHandler()
            => await SongRename();

        [Group("favorite", "Commands to manage favorite songs")]
        public class SongFavoriteInteractionModule : PamelloInteractionModuleBase
        {
            public SongFavoriteInteractionModule(IServiceProvider services) : base(services) { }

            public async Task SongFavoriteAddHandler()
                => await SongFavoriteAdd();
            public async Task SongFavoriteRemoveHandler()
                => await SongFavoriteRemove();
            public async Task SongFavoriteListHandler()
                => await SongFavoriteList();
        }

        [Group("associacions", "Commands to manage song associacions")]
        public class SongAssociacionsInteractionModule : PamelloInteractionModuleBase
        {
            public SongAssociacionsInteractionModule(IServiceProvider services) : base(services) { }

            public async Task SongAssociacionsAddHandler()
                => await SongAssociacionsAdd();
            public async Task SongAssociacionsRemoveHandler()
                => await SongAssociacionsRemove();
            public async Task SongAssociacionsListHandler()
                => await SongAssociacionsList();
        }

        [Group("episodes", "Commands to manage song episodes")]
        public class SongEpisodesInteractionModule : PamelloInteractionModuleBase
        {
            public SongEpisodesInteractionModule(IServiceProvider services) : base(services) { }

            public async Task SongEpisodesAddHandler()
                => await SongEpisodesAdd();
            public async Task SongEpisodesRemoveHandler()
                => await SongEpisodesRemove();
            public async Task SongEpisodesRenameHandler()
                => await SongEpisodesRename();
            public async Task SongEpisodesClearHandler()
                => await SongEpisodesClear();
            public async Task SongEpisodesListHandler()
                => await SongEpisodesList();
        }
    }
}
