using Discord.Interactions;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Modules.Discord.Base;

namespace PamelloV7.Server.Modules.Discord
{
    public enum ESearchSpeakerType
    {
        Discord,
        Internet
    }
    
    [Group("speaker", "Commands to interact with speakers")]
    public class SpeakerInteractionModule : PamelloInteractionModuleBase
    {
        public SpeakerInteractionModule(IServiceProvider services) : base(services) { }
        
        [Group("discord", "Commands for discord speakers interactions")]
        public class SpeakerDiscordInteractionModule : PamelloInteractionModuleBase
        {
            public SpeakerDiscordInteractionModule(IServiceProvider services) : base(services) { }
            
            [SlashCommand("connect", "Connect speaker to your voice channel")]
            public async Task SpeakerDiscordConnectHandler()
                => await SpeakerDiscordConnect();

            [SlashCommand("disconnect", "Disconnect speaker from your voice channel")]
            public async Task SpeakerDiscordDisconnectHandler()
                => await SpeakerDiscordDisconnect();
        }
        
        [Group("internet", "Commands for internet speakers interactions")]
        public class SpeakerInternetInteractionModule : PamelloInteractionModuleBase
        {
            public SpeakerInternetInteractionModule(IServiceProvider services) : base(services) { }
            
            [SlashCommand("connect", "Connect internet speaker")]
            public async Task SpeakerConnectInternetHandler(
                [Summary("name", "Internet speaker public name")] string? name = null
            ) => await SpeakerConnectInternet(name);
        
            [SlashCommand("rename", "Change protection of internet speaker channel")]
            public async Task SpeakerInternetRenameHandler(
                [Summary("speaker", "Internet speaker name/id")] string speakerValue,
                [Summary("new-name", "New public name for the speaker")] string name
            ) => await SpeakerInternetRename(speakerValue, name);
        }

        [SlashCommand("search", "Search in selected player speakers")]
        public async Task SpeakerSearchHandler(
            [Summary("query", "Player list search query")] string query = "",
            [Summary("page", "Page of the players list")] int page = 0,
            [Summary("type", "Type of the search")] ESearchSpeakerType? type = null
        ) => await SpeakerSearch(query, page, type);
    }
}
