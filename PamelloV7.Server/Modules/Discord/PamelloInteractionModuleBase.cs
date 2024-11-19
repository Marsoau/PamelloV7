using Discord;
using Discord.Interactions;
using PamelloV7.Server.Model.Interactions;
using PamelloV7.Server.Model.Interactions.Builders;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Modules.Discord
{
    public class PamelloInteractionModuleBase : InteractionModuleBase<PamelloSocketInteractionContext>
    {
        private readonly UserAuthorizationService _authorization;

        public PamelloInteractionModuleBase(IServiceProvider services)
        {
            _authorization = services.GetRequiredService<UserAuthorizationService>();
        }

        protected async Task ModifyWithEmbedAsync(Embed embed)
        {
            await ModifyOriginalResponseAsync(message => message.Embed = embed);
        }

        //general
        public async Task Ping()
        {
            await ModifyWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Pong!", ""));
        }

        public async Task GetCode()
        {
            await ModifyWithEmbedAsync(PamelloEmbedBuilder.BuildInfo("Authorization Code", _authorization.GetCode(Context.User.DiscordUser.Id).ToString()));
        }

        //Player
        public async Task PlayerSelect()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerCreate()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerDelete()
        {
            throw new NotImplementedException();
        }

        public async Task PlayerResume() {
            throw new NotImplementedException();
        }
        public async Task PlayerPause() {
            throw new NotImplementedException();
        }

        public async Task PlayerSkip()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerGoTo()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerPrev()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerNext()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerGoToEpisode()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerPrevEpisode()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerNextEpisode()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerRewind()
        {
            throw new NotImplementedException();
        }

        public async Task PlayerQueueSongAdd()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueueSongInsert()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueuePlaylistAdd()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueuePlaylistInsert()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueueSongRemove()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueueSongMove()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueueSongSwap()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueueSongRequestNext()
        {
            throw new NotImplementedException();
        }

        public async Task PlayerQueueRandom()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueueReversed()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueueNoLeftovers()
        {
            throw new NotImplementedException();
        }

        public async Task PlayerQueueSuffle()
        {
            throw new NotImplementedException();
        }
        public async Task PlayerQueueClear()
        {
            throw new NotImplementedException();
        }

        //song
        public async Task SongAdd()
        {
            throw new NotImplementedException();
        }
        public async Task SongSearch()
        {
            throw new NotImplementedException();
        }
        public async Task SongInfo()
        {
            throw new NotImplementedException();
        }
        public async Task SongRename()
        {
            throw new NotImplementedException();
        }

        public async Task SongFavoriteAdd()
        {
            throw new NotImplementedException();
        }
        public async Task SongFavoriteRemove()
        {
            throw new NotImplementedException();
        }
        public async Task SongFavoriteList()
        {
            throw new NotImplementedException();
        }

        public async Task SongAssociacionsAdd()
        {
            throw new NotImplementedException();
        }
        public async Task SongAssociacionsRemove()
        {
            throw new NotImplementedException();
        }
        public async Task SongAssociacionsList()
        {
            throw new NotImplementedException();
        }

        public async Task SongEpisodesAdd()
        {
            throw new NotImplementedException();
        }
        public async Task SongEpisodesRemove()
        {
            throw new NotImplementedException();
        }
        public async Task SongEpisodesRename()
        {
            throw new NotImplementedException();
        }
        public async Task SongEpisodesClear()
        {
            throw new NotImplementedException();
        }
        public async Task SongEpisodesList()
        {
            throw new NotImplementedException();
        }

        //playlist
        public async Task PlaylistCreate()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistAddSong()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistAddPlaylistSongs()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistSearch()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistInfo()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistRename()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistFavoriteAdd()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistFavoriteRemove()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistFavoriteList()
        {
            throw new NotImplementedException();
        }
        public async Task PlaylistDelete()
        {
            throw new NotImplementedException();
        }
    }
}
