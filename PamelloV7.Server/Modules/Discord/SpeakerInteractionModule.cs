using Discord.Interactions;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Modules.Discord.Base;

namespace PamelloV7.Server.Modules.Discord
{
    [Group("speaker", "Commands to interact with speakers")]
    public class SpeakerInteractionModule : PamelloInteractionModuleBase
    {
        public SpeakerInteractionModule(IServiceProvider services) : base(services) { }

        [SlashCommand("connect", "Connect speaker to your voice channel")]
        public async Task SpeakerConnectHandler()
            => await SpeakerConnect();

        [SlashCommand("disconnect", "Disconnect speaker from your voice channel")]
        public async Task SpeakerDisconnectHandler()
            => await SpeakerDisconnect();
        
        [SlashCommand("connect-internet", "Connect internet speaker")]
        public async Task SpeakerConnectInternetHandler(
            [Summary("name", "Internet speaker public name")] string? name = null
        ) => await SpeakerConnectInternet(name);
        
        [SlashCommand("rename-internet", "Change protection of internet speaker channel")]
        public async Task SpeakerInternetRenameHandler(
            [Summary("speaker", "Internet speaker name/id")] string speakerValue,
            [Summary("new-name", "New public name for the speaker")] string name
        ) => await SpeakerInternetRename(speakerValue, name);
        
        [SlashCommand("list", "List all speakers available")]
        public async Task SpeakerListHandler()
            => await SpeakerList();
    }
}
