using Discord.Interactions;

namespace PamelloV7.Server.Modules.Discord
{
    [Group("player", "Commands to interact with player")]
    public class PlayerInteractionModule : PamelloInteractionModuleBase
    {
        public PlayerInteractionModule(IServiceProvider services) : base(services) { }

        //Player
        public async Task PlayerSelectHandler()
            => await PlayerSelect();
        public async Task PlayerCreateHandler()
            => await PlayerCreate();
        public async Task PlayerDeleteHandler()
            => await PlayerDelete();

        public async Task PlayerSkipHandler()
            => await PlayerSkip();
        public async Task PlayerGoToHandler()
            => await PlayerGoTo();
        public async Task PlayerPrevHandler()
            => await PlayerPrev();
        public async Task PlayerNextHandler()
            => await PlayerNext();
        public async Task PlayerGoToEpisodeHandler()
            => await PlayerGoToEpisode();
        public async Task PlayerPrevEpisodeHandler()
            => await PlayerPrevEpisode();
        public async Task PlayerNextEpisodeHandler()
            => await PlayerNextEpisode();
        public async Task PlayerRewindHandler()
            => await PlayerRewind();

        [Group("queue", "Commands to interact with selected player queue")]
        public class PlayerQueueInteractionModule : PamelloInteractionModuleBase
        {
            public PlayerQueueInteractionModule(IServiceProvider services) : base(services) { }

            public async Task PlayerQueueSongAddHandler()
                => await PlayerQueueSongAdd();
            public async Task PlayerQueueSongInsertHandler()
                => await PlayerQueueSongInsert();
            public async Task PlayerQueuePlaylistAddHandler()
                => await PlayerQueuePlaylistAdd();
            public async Task PlayerQueuePlaylistInsertHandler()
                => await PlayerQueuePlaylistInsert();
            public async Task PlayerQueueSongRemoveHandler()
                => await PlayerQueueSongRemove();
            public async Task PlayerQueueSongMoveHandler()
                => await PlayerQueueSongMove();
            public async Task PlayerQueueSongSwapHandler()
                => await PlayerQueueSongSwap();
            public async Task PlayerQueueSongRequestNextHandler()
                => await PlayerQueueSongRequestNext();

            public async Task PlayerQueueRandomHandler()
                => await PlayerQueueRandom();
            public async Task PlayerQueueReversedHandler()
                => await PlayerQueueReversed();
            public async Task PlayerQueueNoLeftoversHandler()
                => await PlayerQueueNoLeftovers();

            public async Task PlayerQueueSuffleHandler()
                => await PlayerQueueSuffle();
            public async Task PlayerQueueClearHandler()
                => await PlayerQueueClear();
        }
    }
}
