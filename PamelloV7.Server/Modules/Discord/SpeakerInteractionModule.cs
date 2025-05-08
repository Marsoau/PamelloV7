using Discord.Interactions;
using PamelloV7.Core.Enumerators;

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
            [Summary("channel", "Internet speaker channel name")] string? channel = null,
            [Summary("is-public", "Is speaker available for connection without authorization")] EBoolAnswer isPublic = EBoolAnswer.No
        ) => await SpeakerConnectInternet(channel, isPublic == EBoolAnswer.Yes);
        
        [SlashCommand("channel-protection", "Change protection of internet speaker channel")]
        public async Task SpeakerInternetChangeProtectionHandler(
            [Summary("channel", "Internet speaker channel name")] string channel,
            [Summary("is-public", "Is speaker available for connection without authorization")] EBoolAnswer isPublic
        ) => await SpeakerInternetChangeProtection(channel, isPublic == EBoolAnswer.Yes);
        
        public async Task SpeakerListHandler()
            => await SpeakerList();
    }
}
