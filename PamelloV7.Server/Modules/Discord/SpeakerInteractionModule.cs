using Discord.Interactions;

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
        public async Task SpeakerListHandler()
            => await SpeakerList();
    }
}
