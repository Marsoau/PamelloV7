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

			DiscordClients = new DiscordSocketClient[PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens.Length + 1];

            DiscordClients[0] = services.GetRequiredService<DiscordSocketClient>();
			for (int i = 0; i < PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens.Length; i++) {
				DiscordClients[i + 1] = services.GetRequiredKeyedService<DiscordSocketClient>($"Speaker-{i + 1}");
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

        private async Task Client_UserVoiceStateUpdated(SocketUser discordUser, SocketVoiceState fromVc, SocketVoiceState toVc) {
			if (_speakers is null) return;

			List<PamelloPlayer>? playersFromVc = null;
			List<PamelloPlayer>? playersToVc = null;

			var user = _users.GetByDiscord(discordUser.Id);
			if (user is null) return;

			if (fromVc.VoiceChannel is not null) {
				playersFromVc = _speakers.GetVoicePlayers(fromVc.VoiceChannel.Id);
			}
			if (toVc.VoiceChannel is not null) {
				playersToVc = _speakers.GetVoicePlayers(toVc.VoiceChannel.Id);
			}

			playersFromVc ??= new List<PamelloPlayer>();
			playersToVc ??= new List<PamelloPlayer>();

			if (playersToVc.Count == 1) {
				user.SelectedPlayer = playersToVc.First();
			}
			else if (user.SelectedPlayer is not null && playersFromVc.Contains(user.SelectedPlayer)) {
                user.SelectedPlayer = null;
            }

            Console.WriteLine($"VCC Auto selected player {user.SelectedPlayer?.ToString() ?? "NULL"} for {user}");

			/*
			if (_speakers is null) return;
			if (toVc.VoiceChannel is null) return;

			var pamelloUser = _users.GetByDiscord(user.Id);
			if (pamelloUser is null) return;

			PamelloPlayer? player = null;

            var vcPlayers = _speakers.GetVoicePlayers(toVc.VoiceChannel.Id);
            if (vcPlayers.Count == 1) player = vcPlayers.First();

            Console.WriteLine($"VCC Auto selected player {player?.ToString() ?? "NULL"} for {pamelloUser}");
			pamelloUser.SelectedPlayer = player;
			*/
		}

		public bool IsClientUser(ulong userId) {
			return DiscordClients.Any(client => client.CurrentUser.Id == userId);
		}

        public SocketVoiceChannel? GetUserVoiceChannel(PamelloUser user) {
			SocketVoiceChannel? vc = null;
			foreach (var client in DiscordClients) {
				foreach (var guild in client.Guilds) {
					vc = guild.GetUser(user.DiscordId)?.VoiceChannel;
					if (vc is not null) return vc;
				}
			}

			return null;
		}

        public List<PamelloUser> GetVoiceChannelUsers(SocketVoiceChannel vc) {
			var users = new List<PamelloUser>();

			PamelloUser? pamelloUser = null;
			foreach (var discordUser in vc.ConnectedUsers) {
				pamelloUser = _users.GetByDiscord(discordUser.Id, false);
				if (pamelloUser is null) continue;

				users.Add(pamelloUser);
			}

			return users;
		}

		public SocketUser? GetDiscordUser(ulong userId) {
			SocketUser? user = null;

			foreach (var client in DiscordClients) {
				user = client.GetUser(userId);
				if (user is not null) break;
			}

			return user;
		}
    }
}
