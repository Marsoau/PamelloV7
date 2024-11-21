using Discord.WebSocket;
using PamelloV7.Server.Config;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;

namespace PamelloV7.Server.Services
{
    public class DiscordClientService
    {
		private readonly IServiceProvider _services;

		private PamelloSpeakerService _speakers;

		private PamelloUserRepository _users;

		public DiscordSocketClient[] DiscordClients;

		public DiscordSocketClient MainClient {
			get => DiscordClients[0];
		}

		public DiscordClientService(IServiceProvider services) {
			_services = services;

			var config = services.GetRequiredService<PamelloServerConfig>();

			DiscordClients = new DiscordSocketClient[/* config.SpeakersTokens.Length + */ 2];

            DiscordClients[0] = services.GetRequiredService<DiscordSocketClient>();
			for (int i = 1; i < DiscordClients.Length; i++) {
				DiscordClients[i] = services.GetRequiredKeyedService<DiscordSocketClient>($"Speaker-{i}");
			}
        }

		public void SubscriveToEvents() {
			if (_speakers is not null) return;

			_speakers = _services.GetRequiredService<PamelloSpeakerService>();
			_users = _services.GetRequiredService<PamelloUserRepository>();

			foreach (var client in DiscordClients) {
                client.UserVoiceStateUpdated += Client_UserVoiceStateUpdated;
			}
		}

        private async Task Client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState fromVc, SocketVoiceState toVc) {
			if (_speakers is null) return;

			var pamelloUser = _users.GetByDiscord(user.Id);
			if (pamelloUser is null) return;

			PamelloPlayer? player = null;

			if (toVc.VoiceChannel is not null) {
                var vcPlayers = _speakers.GetVoicePlayers(toVc.VoiceChannel.Id);
                if (vcPlayers.Count == 1) player = vcPlayers.First();
			}

            Console.WriteLine($"VC Auto selected player {player?.ToString() ?? "NULL"} for {pamelloUser}");
			pamelloUser.SelectedPlayer = player;
		}

		public bool IsClientUser(ulong userId) {
			return DiscordClients.Any(client => client.CurrentUser.Id == userId);
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
