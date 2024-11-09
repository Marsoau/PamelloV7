using Discord.WebSocket;
using PamelloV7.Server.Config;

namespace PamelloV7.Server.Services
{
    public class DiscordClientService
    {
		public DiscordSocketClient[] DiscordClients;

		public DiscordSocketClient MainClient {
			get => DiscordClients[0];
		}

		public DiscordClientService(IServiceProvider services) {
			var config = services.GetRequiredService<PamelloServerConfig>();

			DiscordClients = new DiscordSocketClient[/* config.SpeakersTokens.Length */ + 1];

            DiscordClients[0] = services.GetRequiredService<DiscordSocketClient>();
			/*
			for (int i = 0; i < PamelloConfig.SpeakersTokens.Length; i++) {
				DiscordClients[i + 1] = services.GetRequiredKeyedService<DiscordSocketClient>($"Speaker-{i}");
			}
			*/
        }
    }
}
