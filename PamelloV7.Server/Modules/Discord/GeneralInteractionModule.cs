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
            [Summary("song", "Song id/association/name/youtube-url")] string songValue,
            [Summary("position", "Position in queue where song will be added")] int? position = null
        ) => await PlayerQueueSongAdd(songValue, position);

        [SlashCommand("add-playlist", "Add playlist to the queue")]
        public async Task AddPlaylistHandler(
            [Summary("playlist", "Playlist id or name")] string playlistValue,
            [Summary("position", "Position in queue where playlist songs will be added")] int? position = null
        ) => await PlayerQueuePlaylistAdd(playlistValue, position);

        [SlashCommand("get-code", "Get authorization code")]
        public async Task GetCodeHandler()
            => await GetCode();
        
        [SlashCommand("get-client", "Get client download link")]
        public async Task GetClientHandler()
            => await GetClient();
    }
}
