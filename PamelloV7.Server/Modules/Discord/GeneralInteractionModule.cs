using Discord.Interactions;
using PamelloV7.Server.Modules.Discord.Base;

namespace PamelloV7.Server.Modules.Discord
{
    public class GeneralInteractionModule : PamelloInteractionModuleBase
    {
        public GeneralInteractionModule(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("ping", "Ping da bot")]
        public async Task PindHadnler()
            => await Ping();

        [SlashCommand("add", "Add song to the queue")]
        public async Task AddHandler(
            [Summary("song", "Song id/associacion/name/youtube-url")] string songValue
        ) => await PlayerQueueSongAdd(songValue);

        [SlashCommand("add-playlist", "Add playlist to the queue")]
        public async Task AddPlaylistHandler(
            [Summary("playlist", "Playlist id or name")] string playlistValue
        ) => await PlayerQueuePlaylistAdd(playlistValue);

        [SlashCommand("get-code", "Get authorization code")]
        public async Task GetCodeHandler()
            => await GetCode();
    }
}
