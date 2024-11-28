using Discord.Interactions;

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

        [SlashCommand("connect", "Connect speaker to your voice channel")]
        public async Task ConnectHandler()
            => await SpeakerConnect();

        [SlashCommand("disconnect", "Disconnect speaker from your voice channel")]
        public async Task DisconnectHandler()
            => await SpeakerDisconnect();

        [SlashCommand("get-code", "Get authorization code")]
        public async Task GetCodeHandler()
            => await GetCode();

        [SlashCommand("report-problem", "Send problem report")]
        public async Task ReportProblemHandler()
            => await Ping();
    }
}
