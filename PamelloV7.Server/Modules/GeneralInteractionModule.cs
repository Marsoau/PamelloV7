using Discord.Interactions;

namespace PamelloV7.Server.Modules
{
    public class GeneralInteractionModule : PamelloInteractionModuleBase
    {
        public GeneralInteractionModule(IServiceProvider serviceProvider) : base(serviceProvider) {

        }

        [SlashCommand("ping", "Ping da bot")]
        public async Task PindHadnler()
            => await Ping();

        [SlashCommand("add", "Add song to the queue")]
        public async Task AddHandler()
            => await Ping();

        [SlashCommand("add-playlist", "Add playlist songs to the queue")]
        public async Task AddPlaylistHandler()
            => await Ping();

        [SlashCommand("connect", "Connect speaker to your voice channel")]
        public async Task ConnectHandler()
            => await Ping();

        [SlashCommand("disconnect", "Disconnect speaker from your voice channel")]
        public async Task DisconnectHandler()
            => await Ping();

        [SlashCommand("get-code", "Get authorization code")]
        public async Task GetCodeHandler()
            => await GetCode();

        [SlashCommand("report-problem", "Send problem report")]
        public async Task ReportProblemHandler()
            => await Ping();
    }
}
