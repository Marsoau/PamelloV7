using Discord.WebSocket;
using PamelloV7.Server.Config;
using PamelloV7.Server.Model;

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

			DiscordClients = new DiscordSocketClient[/* config.SpeakersTokens.Length + */ 2];

            DiscordClients[0] = services.GetRequiredService<DiscordSocketClient>();
			for (int i = 1; i < DiscordClients.Length; i++) {
				DiscordClients[i] = services.GetRequiredKeyedService<DiscordSocketClient>($"Speaker-{i}");
			}
        }

		public SocketVoiceChannel? GetUserVoiceChannel(PamelloUser user) {
			SocketVoiceChannel? vc = null;
			foreach (var client in DiscordClients) {
				foreach (var guild in client.Guilds) {
					vc = guild.GetUser(user.DiscordUser.Id)?.VoiceChannel;
					if (vc is not null) return vc;
				}
			}

			return null;
		}
    }
}
